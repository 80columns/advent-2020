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

    class Tile {
        public long id;
        public char[,] text;
        public Dictionary<TileSide, BitArray> sides;
        public Dictionary<TileSide, (long id, TileSide side, TileDirection direction)?> matches;
        public Dictionary<TileSide, long?> adjacent_tiles;
        public bool adjacent_tiles_populated;

        public int match_count { get => matches.Count(match => match.Value.HasValue); }

        public Tile(string _tile_text) {
            /*
            Tile 2381:
            #.#.#####.
            #.......#.
            .#.......#
            #.......#.
            ......#..#
            ...#......
            ........##
            ...#..#..#
            ..........
            ..#....#.#
            */

            var tile_parts = _tile_text.Split(':');
            var tile_lines = tile_parts[1].Split("\n", StringSplitOptions.RemoveEmptyEntries);

            this.id = Convert.ToInt64(tile_parts[0].Replace("Tile ", string.Empty).Replace(":", string.Empty));
            this.text = new char[10,10];
            this.sides = new Dictionary<TileSide, BitArray>();
            this.matches = new Dictionary<TileSide, (long id, TileSide side, TileDirection direction)?>();
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

            // initialize adjacent tile pointers
            foreach (var side in new TileSide[] { TileSide.Left, TileSide.Right, TileSide.Top, TileSide.Bottom }) {
                this.adjacent_tiles.Add(side, null);
            }

            this.adjacent_tiles_populated = false;
        }
    
        public void PopulateMatchingSides(Tile _other) {
            var other_side_matches = new Dictionary<TileSide, Dictionary<(TileSide other_side, TileDirection other_direction), int>>() {
                [TileSide.Left] = new Dictionary<(TileSide other_side, TileDirection other_direction), int>(),
                [TileSide.Right] = new Dictionary<(TileSide other_side, TileDirection other_direction), int>(),
                [TileSide.Top] = new Dictionary<(TileSide other_side, TileDirection other_direction), int>(),
                [TileSide.Bottom] = new Dictionary<(TileSide other_side, TileDirection other_direction), int>()
            };

            foreach (var side1 in other_side_matches.Keys) {
                foreach (var side2 in other_side_matches.Keys) {
                    other_side_matches[side1].Add((side2, TileDirection.Normal), 0);
                    other_side_matches[side1].Add((side2, TileDirection.Reversed), 0);
                }
            }

            // compare current sides to other sides (normal + reversed direction)
            for (var i = 0; i < 10; i++) {
                foreach (var side in new TileSide[] { TileSide.Left, TileSide.Right, TileSide.Top, TileSide.Bottom }) {
                    foreach (var other_side in new TileSide[] { TileSide.Left, TileSide.Right, TileSide.Top, TileSide.Bottom }) {
                        if (this.sides[side][i] == _other.sides[other_side][i]) {
                            other_side_matches[side][(other_side, TileDirection.Normal)] += 1;
                        }

                        if (this.sides[side][i] == _other.sides[other_side][9 - i]) {
                            other_side_matches[side][(other_side, TileDirection.Reversed)] += 1;
                        }
                    }
                }
            }

            foreach (var side in new TileSide[] { TileSide.Left, TileSide.Right, TileSide.Top, TileSide.Bottom }) {
                foreach (var other_side in new TileSide[] { TileSide.Left, TileSide.Right, TileSide.Top, TileSide.Bottom }) {
                    if (other_side_matches[side][(other_side, TileDirection.Normal)] == 10) {
                        this.matches.Add(side, (_other.id, other_side, TileDirection.Normal));
                    }

                    if (other_side_matches[side][(other_side, TileDirection.Reversed)] == 10) {
                        this.matches.Add(side, (_other.id, other_side, TileDirection.Reversed));
                    }
                }
            }
        }

        // flip tile, reversing top & bottom
        public void FlipTopBottom() {
            // flip the tile's text vertically
            var temp_text = new char[,] { this.text };

            for (var i = temp_text.GetLength(0) - 1; i >= 0; i--) {

            }

            // swap the top & bottom sides of the array
            var top_array = this.sides[TileSide.Top];
            var bottom_array = this.sides[TileSide.Bottom];

            this.sides[TileSide.Top] = bottom_array;
            this.sides[TileSide.Bottom] = top_array;

            // flip the direction of the left/right tile matches
            foreach (var match_side in this.matches.Keys) {
                if (match_side == TileSide.Left) {
                    if (this.matches[match_side].Value.direction == TileDirection.Normal) {
                        this.matches[match_side] = (this.matches[match_side].Value.id, this.matches[match_side].Value.side, TileDirection.Reversed);
                    } else {
                        this.matches[match_side] = (this.matches[match_side].Value.id, this.matches[match_side].Value.side, TileDirection.Normal);
                    }
                } else if (match_side == TileSide.Right) {

                }
            }
        }

        public void FlipLeftRight() {
            // flip tile, reversing left & right

        }
    }

    class Program {
        static async Task Main(string[] args) {
            var input = await File.ReadAllTextAsync("input.txt", Encoding.UTF8);
            var tile_texts = input.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
            var tiles = new Dictionary<long, Tile>();
            var multiplication_result = (long)1;

            foreach (var tile_text in tile_texts) {
                var new_tile = new Tile(tile_text);

                tiles.Add(new_tile.id, new_tile);
            }

            foreach (var tile in tiles) {
                foreach (var other_tile in tiles.Where(t => t.Key != tile.Key)) {
                    tile.Value.PopulateMatchingSides(other_tile.Value);
                }
            }

            /*
             * Part 1
             *
             * Find the corners of the image
             */
            foreach (var tile in tiles.Where(tile => tile.Value.match_count == 2)) {
                multiplication_result *= tile.Key;
            }

            Console.WriteLine(multiplication_result);

            /*
             * Part 2
             *
             * Find the monsters in the image
             */
            var temp_tile_id = tiles.First(tile => tile.Value.match_count == 2).Key;



            while (!tiles[temp_tile_id].adjacent_tiles_populated) {
                Console.WriteLine($"processing tile {temp_tile_id}...");

                // get matches for non-populated adjacent tiles
                var matches = tiles[temp_tile_id].matches.Where(match => match.Value.HasValue && !tiles[temp_tile_id].adjacent_tiles[match.Key].HasValue);

                foreach (var match in matches) {
                    if (
                        match.Value.Value.direction == TileDirection.Normal
                     && (match.Key == TileSide.Left && match.Value.Value.side == TileSide.Right)
                     && (match.Key == TileSide.Right && match.Value.Value.side == TileSide.Left)
                     && (match.Key == TileSide.Top && match.Value.Value.side == TileSide.Bottom)
                     && (match.Key == TileSide.Bottom && match.Value.Value.side == TileSide.Top)
                    ) {
                        // default case where the adjacent tile does not need to be flipped or rotated
                        tiles[temp_tile_id].adjacent_tiles.Add(match.Key, match.Value.Value.id);
                        tiles[match.Value.Value.id].adjacent_tiles.Add(match.Value.Value.side, temp_tile_id);
                    } else if (
                        match.Value.Value.direction == TileDirection.Reversed
                     && (match.Key == TileSide.Left && match.Value.Value.side == TileSide.Right)
                     && (match.Key == TileSide.Right && match.Value.Value.side == TileSide.Left)
                    ) {
                        // flip adjacent tile, reversing top & bottom
                        tiles[match.Value.Value.id].FlipTopBottom();

                        // reverse the left/right tile direction of the adjacent tile's matching tiles


                        tiles[temp_tile_id].adjacent_tiles.Add(match.Key, match.Value.Value.id);
                        tiles[match.Value.Value.id].adjacent_tiles.Add(match.Value.Value.side, temp_tile_id);
                    } else if (
                        match.Value.Value.direction == TileDirection.Reversed
                     && (match.Key == TileSide.Top && match.Value.Value.side == TileSide.Bottom)
                     && (match.Key == TileSide.Bottom && match.Value.Value.side == TileSide.Top)
                    ) {
                        // flip adjacent tile, reversing left & right
                        tiles[match.Value.Value.id].FlipLeftRight();

                        tiles[temp_tile_id].adjacent_tiles.Add(match.Key, match.Value.Value.id);
                        tiles[match.Value.Value.id].adjacent_tiles.Add(match.Value.Value.side, temp_tile_id);
                    } else if () {
                        // rotate adjacent tile left

                    } else if () {
                        // rotate adjacent tile right

                    }
                }

                tiles[temp_tile_id].adjacent_tiles_populated = true;
            }

            Console.WriteLine("Hello World!");
        }
    }
}