using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20 {
    enum TileSide {
        Left,
        Right,
        Top,
        Bottom
    }

    enum TileDirection {
        Normal,
        Reversed
    }

    static class ArrayExtensions {
        public static void Print(this char[,] _array) {
            for (var i = 0; i < _array.GetLength(0); i++) {
                for (var j = 0; j < _array.GetLength(1); j++) {
                    Console.Write(_array[i, j]);
                }

                Console.WriteLine();
            }
        }
    }

    static class TileSideExtensions {
        public static bool IsOpposite(this TileSide _side, TileSide _other_side) {
            return (_side == TileSide.Left && _other_side == TileSide.Right)
                || (_side == TileSide.Right && _other_side == TileSide.Left)
                || (_side == TileSide.Top && _other_side == TileSide.Bottom)
                || (_side == TileSide.Bottom && _other_side == TileSide.Top);
        }

        public static bool IsVerticallySame(this TileSide _side, TileSide _other_side) {
            return (_side == _other_side)
                && (_side == TileSide.Top || _side == TileSide.Bottom);
        }

        public static bool IsHorizontallySame(this TileSide _side, TileSide _other_side) {
            return (_side == _other_side)
                && (_side == TileSide.Left || _side == TileSide.Right);
        }

        public static TileSide GetOppositeSide(this TileSide _side) {
            if (_side == TileSide.Left) {
                return TileSide.Right;
            } else if (_side == TileSide.Right) {
                return TileSide.Left;
            } else if (_side == TileSide.Top) {
                return TileSide.Bottom;
            } else {
                return TileSide.Top;
            }
        }
    
        public static bool ValueEquals(this BitArray _side, BitArray _other_side) {
            var equal = true;

            // for the purposes of this program, both bit array objects are assumed to have the same length
            for (var i = 0; i < 10; i++) {
                if (_side[i] != _other_side[i]) {
                    equal = false;
                    break;
                }
            }

            return equal;
        }

        public static bool ReversedValueEquals(this BitArray _side, BitArray _other_side) {
            var equal = true;

            // for the purposes of this program, both bit array objects are assumed to have the same length
            for (var i = 0; i < 10; i++) {
                if (_side[i] != _other_side[9 - i]) {
                    equal = false;
                    break;
                }
            }

            return equal;
        }
    }

    class Tile {
        /*
        Tile x:
          0123456789
        0 #.#.#####. 0
        1 #.......#. 1
        2 .#.......# 2
        3 #.......#. 3
        4 ......#..# 4
        5 ...#...... 5
        6 ........## 6
        7 ...#..#..# 7
        8 .......... 8
        9 ..#....#.# 9
          0123456789
        */

        public long id;
        private char[,] text;
        public char[,] borderless_text {
            get {
                var temp_text = new char[8,8];

                for (int i = 0, j = 1; i < 8; i++, j++) {
                    for (int k = 0, l = 1; k < 8; k++, l++) {
                        temp_text[i, k] = this.text[j, l];
                    }
                }

                return temp_text;
            }
        }
        private Dictionary<TileSide, BitArray> sides;
        public Dictionary<TileSide, long?> adjacent_tiles;

        public Tile(string _tile_text) {
            var tile_parts = _tile_text.Split(':');
            var tile_lines = tile_parts[1].Split("\n", StringSplitOptions.RemoveEmptyEntries);

            this.id = Convert.ToInt64(tile_parts[0].Replace("Tile ", string.Empty).Replace(":", string.Empty));
            this.text = new char[10,10];
            this.sides = new Dictionary<TileSide, BitArray>();
            this.adjacent_tiles = new Dictionary<TileSide, long?>();

            // populate left and right sides
            this.sides.Add(TileSide.Left, new BitArray(10));
            this.sides.Add(TileSide.Right, new BitArray(10));

            for (var i = 0; i < 10; i++) {
                this.sides[TileSide.Left][i] = tile_lines[i][0] == '#';
                this.sides[TileSide.Right][i] = tile_lines[i][9] == '#';
            }

            // populate top and bottom sides
            this.sides.Add(TileSide.Top, new BitArray(10));
            this.sides.Add(TileSide.Bottom, new BitArray(10));

            for (var i = 0; i < 10; i++) {
                this.sides[TileSide.Top][i] = tile_lines[0][i] == '#';
                this.sides[TileSide.Bottom][i] = tile_lines[9][i] == '#';
            }

            // store tile text
            for (var i = 0; i < 10; i++) {
                for (var j = 0; j < 10; j++) {
                    this.text[i, j] = tile_lines[i][j];
                }
            }
        }
    
        public BitArray Side(TileSide _side) {
            return this.sides[_side];
        }

        public void FlipVertically() {
            // flip the sides vertically
            var new_top_side = new BitArray(this.sides[TileSide.Bottom]);
            var new_bottom_side = new BitArray(this.sides[TileSide.Top]);
            var new_left_side = new BitArray(10);
            var new_right_side = new BitArray(10);

            for (var i = 0; i < 10; i++) {
                new_left_side[i] = this.sides[TileSide.Left][9 - i];
                new_right_side[i] = this.sides[TileSide.Right][9 - i];
            }

            this.sides.Clear();

            this.sides.Add(TileSide.Left, new_left_side);
            this.sides.Add(TileSide.Right, new_right_side);
            this.sides.Add(TileSide.Top, new_top_side);
            this.sides.Add(TileSide.Bottom, new_bottom_side);

            // flip the text vertically
            var temp_text = new char[10,10];

            for (var i = 0; i < 10; i++) {
                for (var j = 0; j < 10; j++) {
                    temp_text[9 - i, j] = this.text[i, j];
                }
            }

            this.text = temp_text;
        }

        public void FlipHorizontally() {
            // flip the sides horizontally
            var new_left_side = new BitArray(this.sides[TileSide.Right]);
            var new_right_side = new BitArray(this.sides[TileSide.Left]);
            var new_top_side = new BitArray(10);
            var new_bottom_side = new BitArray(10);

            for (var i = 0; i < 10; i++) {
                new_top_side[i] = this.sides[TileSide.Top][9 - i];
                new_bottom_side[i] = this.sides[TileSide.Bottom][9 - i];
            }

            this.sides.Clear();

            this.sides.Add(TileSide.Left, new_left_side);
            this.sides.Add(TileSide.Right, new_right_side);
            this.sides.Add(TileSide.Top, new_top_side);
            this.sides.Add(TileSide.Bottom, new_bottom_side);

            // flip the text horizontally
            var temp_text = new char[10, 10];

            for (var i = 0; i < 10; i++) {
                for (var j = 0; j < 10; j++) {
                    temp_text[i, 9 - j] = this.text[i, j];
                }
            }

            this.text = temp_text;
        }
    
        public void RotateLeft() {
            // rotate the sides left
            var new_left_side = new BitArray(10);
            var new_right_side = new BitArray(10);
            var new_top_side = new BitArray(this.sides[TileSide.Right]);
            var new_bottom_side = new BitArray(this.sides[TileSide.Left]);

            for (var i = 0; i < 10; i++) {
                new_left_side[i] = this.sides[TileSide.Top][9 - i];
                new_right_side[i] = this.sides[TileSide.Bottom][9 - i];
            }

            this.sides.Clear();

            this.sides.Add(TileSide.Left, new_left_side);
            this.sides.Add(TileSide.Right, new_right_side);
            this.sides.Add(TileSide.Top, new_top_side);
            this.sides.Add(TileSide.Bottom, new_bottom_side);

            // rotate the text left
            var temp_text = new char[10, 10];

            for (var i = 0; i < 10; i++) {
                for (var j = 0; j < 10; j++) {
                    temp_text[9 - j, i] = this.text[i, j];
                }
            }

            this.text = temp_text;
        }

        public void RotateRight() {
            // rotate the sides right
            var new_left_side = new BitArray(this.sides[TileSide.Bottom]);
            var new_right_side = new BitArray(this.sides[TileSide.Top]);
            var new_top_side = new BitArray(10);
            var new_bottom_side = new BitArray(10);

            for (var i = 0; i < 10; i++) {
                new_top_side[i] = this.sides[TileSide.Left][9 - i];
                new_bottom_side[i] = this.sides[TileSide.Right][9 - i];
            }

            this.sides.Clear();

            this.sides.Add(TileSide.Left, new_left_side);
            this.sides.Add(TileSide.Right, new_right_side);
            this.sides.Add(TileSide.Top, new_top_side);
            this.sides.Add(TileSide.Bottom, new_bottom_side);

            // rotate the text right
            var temp_text = new char[10, 10];

            for (var i = 0; i < 10; i++) {
                for (var j = 0; j < 10; j++) {
                    temp_text[j, 9 - i] = this.text[i, j];
                }
            }

            this.text = temp_text;
        }
    
        public void Print() {
            Console.WriteLine($"{this.id}:");

            this.text.Print();
        }
    }

    class Grid {
        private Dictionary<long, Tile> tiles;
        private List<Tile> unmatched_tiles;
        private char[,] image_text;

        public Grid() {
            this.tiles = new Dictionary<long, Tile>();
            this.unmatched_tiles = new List<Tile>();
            this.image_text = null;
        }

        public void AddTile(Tile _new_tile) {
            if (this.MatchTileToGrid(ref _new_tile)) {
                this.tiles.Add(_new_tile.id, _new_tile);
            } else {
                this.unmatched_tiles.Add(_new_tile);
            }
        }

        public void ResolveUnmatchedTiles() {
            while (this.unmatched_tiles.Any()) {
                for (var i = this.unmatched_tiles.Count - 1; i >= 0; i--) {
                    var temp_tile = this.unmatched_tiles[i];

                    if (this.MatchTileToGrid(ref temp_tile)) {
                        this.tiles.Add(temp_tile.id, temp_tile);
                        this.unmatched_tiles.RemoveAt(i);
                    }
                }
            }
        }

        public void ResolveAdjacentTiles() {
            var all_sides = new HashSet<TileSide>() { TileSide.Left, TileSide.Right, TileSide.Top, TileSide.Bottom };

            foreach (var tile_id in this.tiles.Keys) {
                // if this tile's adjacent tiles list is possibly incomplete, attempt to match the remaining ones
                if (this.tiles[tile_id].adjacent_tiles.Count < 4) {
                    foreach (var tile_side in all_sides.Except(this.tiles[tile_id].adjacent_tiles.Keys)) {
                        foreach (var other_tile_id in this.tiles.Keys.Where(key => key != tile_id)) {
                            if (this.tiles[tile_id].Side(tile_side).ValueEquals(this.tiles[other_tile_id].Side(tile_side.GetOppositeSide()))) {
                                this.tiles[tile_id].adjacent_tiles.Add(tile_side, other_tile_id);
                                this.tiles[other_tile_id].adjacent_tiles.Add(tile_side.GetOppositeSide(), tile_id);

                                break;
                            }
                        }
                    }
                }
            }
        }

        private bool MatchTileToGrid(ref Tile _new_tile) {
            var new_tile_matched = false;

            if (this.tiles.Any()) {
                foreach (var tile_id in this.tiles.Keys) {
                    // find a matching tile in the current tile dictionary, then flip/rotate the new tile to fit the grid
                    foreach (var new_tile_side in new TileSide[] { TileSide.Left, TileSide.Right, TileSide.Top, TileSide.Bottom }) {
                        foreach (var existing_tile_side in new TileSide[] { TileSide.Left, TileSide.Right, TileSide.Top, TileSide.Bottom }) {
                            if (_new_tile.Side(new_tile_side).ValueEquals(this.tiles[tile_id].Side(existing_tile_side))) {
                                // found a match, now flip/rotate the new tile and assign adjacent tiles
                                new_tile_matched = true;
                                
                                if (new_tile_side.IsOpposite(existing_tile_side)) {
                                    // if the sides are opposites & they align as-is, then no flipping/rotation is necessary and they can be assigned directly as adjacent tiles
                                } else if (new_tile_side.IsVerticallySame(existing_tile_side)) {
                                    // if the sides are the same & they align top-top or bottom-bottom, then flip the new tile vertically
                                    _new_tile.FlipVertically();
                                } else if (new_tile_side.IsHorizontallySame(existing_tile_side)) {
                                    // if the sides are the same & they align left-left or right-right, then flip the new tile horizontally
                                    _new_tile.FlipHorizontally();
                                } else if (
                                    (new_tile_side == TileSide.Left && existing_tile_side == TileSide.Top)
                                || (new_tile_side == TileSide.Right && existing_tile_side == TileSide.Bottom)
                                ) {
                                    // rotate the new tile left so the left becomes the bottom
                                    // OR
                                    // rotate the new tile left so the right becomes the top
                                    _new_tile.RotateLeft();
                                } else if (
                                    (new_tile_side == TileSide.Left && existing_tile_side == TileSide.Bottom)
                                || (new_tile_side == TileSide.Right && existing_tile_side == TileSide.Top)
                                ) {
                                    // rotate the new tile right so the left becomes the top, then flip horizontally to match correctly
                                    // OR
                                    // rotate the new tile right so the right becomes the bottom, then flip horizontally to match correctly
                                    _new_tile.RotateRight();
                                    _new_tile.FlipHorizontally();
                                } else if (
                                    (new_tile_side == TileSide.Top && existing_tile_side == TileSide.Left)
                                || (new_tile_side == TileSide.Bottom && existing_tile_side == TileSide.Right)
                                ) {
                                    // rotate the new tile right so the top becomes the right
                                    // OR
                                    // rotate the new tile right so the bottom becomes the left
                                    _new_tile.RotateRight();
                                } else if (
                                    (new_tile_side == TileSide.Top && existing_tile_side == TileSide.Right)
                                || (new_tile_side == TileSide.Bottom && existing_tile_side == TileSide.Left)
                                ) {
                                    // rotate the new tile left so the top becomes the left, then flip vertically to match correctly
                                    // OR
                                    // rotate the new tile left so the bottom becomes the right, then flip vertically to match correctly
                                    _new_tile.RotateLeft();
                                    _new_tile.FlipVertically();
                                }
                            } else if (_new_tile.Side(new_tile_side).ReversedValueEquals(this.tiles[tile_id].Side(existing_tile_side))) {
                                // found a match, now flip/rotate the new tile and assign adjacent tiles
                                new_tile_matched = true;

                                if (new_tile_side.IsOpposite(existing_tile_side)) {
                                    // if the sides are opposites & they align reversed, then flip the new tile vertically or horizontally depending on which sides match
                                    if (new_tile_side == TileSide.Left || new_tile_side == TileSide.Right) {
                                        _new_tile.FlipVertically();
                                    } else {
                                        _new_tile.FlipHorizontally();
                                    }
                                } else if (new_tile_side.IsVerticallySame(existing_tile_side)) {
                                    // if the sides are the same & they align top-top or bottom-bottom reversed, then flip the new tile vertically and then horizontally
                                    _new_tile.FlipVertically();
                                    _new_tile.FlipHorizontally();
                                } else if (new_tile_side.IsHorizontallySame(existing_tile_side)) {
                                    // if the sides are the same & they align left-left or right-right reversed, then flip the new tile horizontally and then vertically
                                    _new_tile.FlipHorizontally();
                                    _new_tile.FlipVertically();
                                } else if (
                                    (new_tile_side == TileSide.Left && existing_tile_side == TileSide.Top)
                                || (new_tile_side == TileSide.Right && existing_tile_side == TileSide.Bottom)
                                ) {
                                    // rotate the new tile left so the left becomes the bottom, then flip horizontally to match correctly
                                    // OR
                                    // rotate the new tile left so the right becomes the top, then flip horizontally to match correctly
                                    _new_tile.RotateLeft();
                                    _new_tile.FlipHorizontally();
                                } else if (
                                    (new_tile_side == TileSide.Left && existing_tile_side == TileSide.Bottom)
                                || (new_tile_side == TileSide.Right && existing_tile_side == TileSide.Top)
                                ) {
                                    // rotate the new tile right so the left becomes the top
                                    // OR
                                    // rotate the new tile right so the right becomes the bottom
                                    _new_tile.RotateRight();
                                } else if (
                                    (new_tile_side == TileSide.Top && existing_tile_side == TileSide.Left)
                                || (new_tile_side == TileSide.Bottom && existing_tile_side == TileSide.Right)
                                ) {
                                    // rotate the new tile right so the top becomes the right, then flip vertically to match correctly
                                    // OR
                                    // rotate the new tile right so the bottom becomes the left, then flip vertically to match correctly
                                    _new_tile.RotateRight();
                                    _new_tile.FlipVertically();
                                } else if (
                                    (new_tile_side == TileSide.Top && existing_tile_side == TileSide.Right)
                                || (new_tile_side == TileSide.Bottom && existing_tile_side == TileSide.Left)
                                ) {
                                    // rotate the new tile left so the top becomes the left
                                    // OR
                                    // rotate the new tile left so the bottom becomes the right
                                    _new_tile.RotateLeft();
                                }
                            }

                            if (new_tile_matched) {
                                _new_tile.adjacent_tiles.Add(existing_tile_side.GetOppositeSide(), tile_id);
                                this.tiles[tile_id].adjacent_tiles.Add(existing_tile_side, _new_tile.id);

                                break;
                            }
                        }

                        if (new_tile_matched) {
                            break;
                        }
                    }

                    if (new_tile_matched) {
                        break;
                    }
                }
            } else {
                // set to true automatically as the new tile is the first one
                new_tile_matched = true;
            }

            return new_tile_matched;
        }

        public long ComputeCornerIdProduct() {
            var product = (long)1;

            foreach (var tile in this.tiles.Where(tile => tile.Value.adjacent_tiles.Count == 2)) {
                product *= tile.Key;
            }

            return product;
        }

        public void PopulateImage() {
            var tiles_processed = 0;
            var image_row_index = 0;
            var image_column_index = 0;
            var chars_per_image_side = (int)Math.Sqrt(this.tiles.Count) * 8;

            this.image_text = new char[chars_per_image_side, chars_per_image_side];

            // build the image starting with the top left tile
            var current_tile_id = this.tiles.Where(tile => tile.Value.adjacent_tiles.Count == 2 && tile.Value.adjacent_tiles.ContainsKey(TileSide.Bottom) && tile.Value.adjacent_tiles.ContainsKey(TileSide.Right)).First().Key;

            while (tiles_processed < this.tiles.Count) {
                while (image_column_index < chars_per_image_side) {
                    // add the borderless text from the current tile to the image text
                    var borderless_text = this.tiles[current_tile_id].borderless_text;

                    for (var i = 0; i < 8; i++, image_row_index++) {
                        for (var j = 0; j < 8; j++, image_column_index++) {
                            this.image_text[image_row_index, image_column_index] = borderless_text[i, j];
                        }

                        image_column_index -= 8;
                    }

                    // reset the row index of the image, and move the column index to the right
                    image_row_index -= 8;
                    image_column_index += 8;

                    tiles_processed++;

                    // move to the next tile on the right, if this isn't the last one
                    if (this.tiles[current_tile_id].adjacent_tiles.ContainsKey(TileSide.Right)) {
                        current_tile_id = this.tiles[current_tile_id].adjacent_tiles[TileSide.Right].Value;
                    }
                }

                image_row_index += 8;
                image_column_index = 0;

                // go back to the left of the grid, then to the 1st left tile 1 row down
                while (this.tiles[current_tile_id].adjacent_tiles.ContainsKey(TileSide.Left)) {
                    current_tile_id = this.tiles[current_tile_id].adjacent_tiles[TileSide.Left].Value;
                }

                // if this tile has a tile beneath it, then start from that row next
                if (this.tiles[current_tile_id].adjacent_tiles.ContainsKey(TileSide.Bottom)) {
                    current_tile_id = this.tiles[current_tile_id].adjacent_tiles[TileSide.Bottom].Value;
                }
            }
        }

        public int GetSeaMonsterCount() {
            var sea_monster_count = 0;

            /* search for sea monsters:

                                 # 
               #    ##    ##    ###
                #  #  #  #  #  #   

               [i, j] = #

               [i-18, j+1] = #
               [i-13, j+1] = #
               [i-12, j+1] = #
               [i-7, j+1] = #
               [i-6, j+1] = #
               [i-1, j+1] = #
               [i, j+1] = #
               [i+1, j+1] = #

               [i-17, j+2] = #
               [i-14, j+2] = #
               [i-11, j+2] = #
               [i-8, j+2] = #
               [i-5, j+2] = #
               [i-2, j+2] = #
             */

            void FindSeaMonsters() {
                for (var i = 0; i < this.image_text.GetLength(0); i++) {
                    for (var j = 0; j < this.image_text.GetLength(1); j++) {
                        if (
                            i - 18 >= 0
                        && i + 1 < this.image_text.GetLength(0)
                        && j + 2 < this.image_text.GetLength(1)
                        && this.image_text[i, j] == '#'
                        && this.image_text[i-18, j+1] == '#'
                        && this.image_text[i-13, j+1] == '#'
                        && this.image_text[i-12, j+1] == '#'
                        && this.image_text[i-7, j+1] == '#'
                        && this.image_text[i-6, j+1] == '#'
                        && this.image_text[i-1, j+1] == '#'
                        && this.image_text[i, j+1] == '#'
                        && this.image_text[i+1, j+1] == '#'
                        && this.image_text[i-17, j+2] == '#'
                        && this.image_text[i-14, j+2] == '#'
                        && this.image_text[i-11, j+2] == '#'
                        && this.image_text[i-8, j+2] == '#'
                        && this.image_text[i-5, j+2] == '#'
                        && this.image_text[i-2, j+2] == '#'
                        ) {
                            sea_monster_count++;
                        }
                    }
                }
            }

            // search for sea monsters using the original image orientation
            FindSeaMonsters();

            for (var i = 0; i < 3; i++) {
                if (sea_monster_count == 0) {
                    // rotate the image left, then search again
                    this.RotateImageLeft();
                    FindSeaMonsters();
                } else {
                    break;
                }
            }

            if (sea_monster_count == 0) {
                // rotate the image left to the original image orientation
                // flip the image horizontally
                this.RotateImageLeft();
                this.FlipImageHorizontally();
                FindSeaMonsters();
            }

            for (var i = 0; i < 3; i++) {
                if (sea_monster_count == 0) {
                    // rotate the image left, then search again
                    this.RotateImageLeft();
                    FindSeaMonsters();
                } else {
                    break;
                }
            }

            if (sea_monster_count == 0) {
                // rotate the image left to the original horizontally-flipped image orientation
                // flip the image horizontally to the original image orientation
                // flip the image vertically
                this.RotateImageLeft();
                this.FlipImageHorizontally();
                this.FlipImageVertically();
                FindSeaMonsters();
            }

            for (var i = 0; i < 3; i++) {
                if (sea_monster_count == 0) {
                    // rotate the image left, then search again
                    this.RotateImageLeft();
                    FindSeaMonsters();
                } else {
                    break;
                }
            }

            if (sea_monster_count == 0) {
                // rotate the image left to the original vertically-flipped image orientation
                // flip the image horizontally (to a vertical-horizontal flip orientation)
                this.RotateImageLeft();
                this.FlipImageHorizontally();
            }

            for (var i = 0; i < 3; i++) {
                if (sea_monster_count == 0) {
                    // rotate the image left, then search again
                    this.RotateImageLeft();
                    FindSeaMonsters();
                } else {
                    break;
                }
            }

            return sea_monster_count;
        }

        private void FlipImageVertically() {
            var temp_image = new char[this.image_text.GetLength(0), this.image_text.GetLength(1)];

            for (var i = 0; i < this.image_text.GetLength(0); i++) {
                for (var j = 0; j < this.image_text.GetLength(1); j++) {
                    temp_image[this.image_text.GetLength(0) - 1 - i, j] = this.image_text[i, j];
                }
            }

            this.image_text = temp_image;
        }

        private void FlipImageHorizontally() {
            var temp_image = new char[this.image_text.GetLength(0), this.image_text.GetLength(1)];

            for (var i = 0; i < this.image_text.GetLength(0); i++) {
                for (var j = 0; j < this.image_text.GetLength(1); j++) {
                    temp_image[i, this.image_text.GetLength(1) - 1 - j] = this.image_text[i, j];
                }
            }

            this.image_text = temp_image;
        }

        private void RotateImageLeft() {
            var temp_image = new char[this.image_text.GetLength(0), this.image_text.GetLength(1)];

            for (var i = 0; i < this.image_text.GetLength(0); i++) {
                for (var j = 0; j < this.image_text.GetLength(1); j++) {
                    temp_image[this.image_text.GetLength(1) - 1 - j, i] = this.image_text[i, j];
                }
            }

            this.image_text = temp_image;
        }

        private void RotateImageRight() {
            var temp_image = new char[this.image_text.GetLength(0), this.image_text.GetLength(1)];

            for (var i = 0; i < this.image_text.GetLength(0); i++) {
                for (var j = 0; j < this.image_text.GetLength(1); j++) {
                    temp_image[j, this.image_text.GetLength(0) - 1 - i] = this.image_text[i, j];
                }
            }

            this.image_text = temp_image;
        }

        public int GetImageHashCharCount() {
            var hash_char_count = 0;

            for (var i = 0; i < this.image_text.GetLength(0); i++) {
                for (var j = 0; j < this.image_text.GetLength(1); j++) {
                    if (this.image_text[i, j] == '#') {
                        hash_char_count++;
                    }
                }
            }

            return hash_char_count;
        }

        public Tile this[long index] {
            get => this.tiles[index];
        }
    }

    class Program {
        static async Task Main(string[] args) {
            var input = await File.ReadAllTextAsync("input.txt", Encoding.UTF8);
            var tile_texts = input.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);

            /*
             * Part 1
             *
             * Find the corners of the image
             */
            var grid = new Grid();

            foreach (var tile_text in tile_texts) {
                grid.AddTile(new Tile(tile_text));
            }

            grid.ResolveUnmatchedTiles();
            grid.ResolveAdjacentTiles();

            Console.WriteLine(grid.ComputeCornerIdProduct());

            /*
             * Part 2
             *
             * Find the monsters in the image,
             * then the count of # in the image that are not part of a sea monster
             */
            grid.PopulateImage();

            var sea_monster_count = grid.GetSeaMonsterCount();
            var hash_char_count = grid.GetImageHashCharCount();

            Console.WriteLine(hash_char_count - (sea_monster_count * 15));
        }
    }
}