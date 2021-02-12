using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _24 {
    enum TileColor {
        Black,
        White
    }

    class Tile {
        public TileColor color { get; private set; }
        private bool conditionally_flip;

        public void Flip() {
            this.color = (this.color == TileColor.White) ? TileColor.Black : TileColor.White;
        }

        public void EvaluateConditionalFlip(List<TileColor> neighbor_colors) {
            if (this.color == TileColor.Black) {
                if (
                    neighbor_colors.Any(color => color == TileColor.Black) == false
                 || neighbor_colors.Count(color => color == TileColor.Black) > 2
                ) {
                    this.conditionally_flip = true;
                }
            } else if (this.color == TileColor.White) {
                if (neighbor_colors.Count(color => color == TileColor.Black) == 2) {
                    this.conditionally_flip = true;
                }
            }
        }

        public void ConditionalFlip() {
            if (this.conditionally_flip) {
                this.Flip();
                this.conditionally_flip = false;
            }
        }

        public Tile() {
            this.color = TileColor.White;
            this.conditionally_flip = false;
        }
    }

    class Program {
        static async Task Main(string[] args) {
            var tile_instructions = await File.ReadAllLinesAsync("input.txt", Encoding.UTF8);
            var tiles = new Dictionary<(int x, int y), Tile>() { [(0, 0)] = new Tile() };

            foreach (var tile_instruction in tile_instructions) {
                var current_instruction = new StringBuilder();
                var current_x = 0;
                var current_y = 0;

                for (var instruction_index = 0; instruction_index < tile_instruction.Length; instruction_index++) {
                    current_instruction.Append(tile_instruction[instruction_index]);

                    // tile = (x, y)
                    // east => (x + 1, y)
                    // west => (x - 1, y)
                    // northwest => (x, y + 1)
                    // northeast => (x + 1, y + 1)
                    // southwest => (x - 1, y - 1)
                    // southeast => (x, y - 1)
                    switch (tile_instruction[instruction_index]) {
                        case 'e':
                            current_x++;

                            break;

                        case 'w':
                            current_x--;

                            break;

                        case 'n':
                            current_instruction.Append(tile_instruction[instruction_index+1]);

                            switch (tile_instruction[instruction_index+1]) {
                                case 'w':
                                    current_y++;

                                    break;

                                case 'e':
                                    current_x++;
                                    current_y++;

                                    break;
                            }

                            instruction_index++;

                            break;

                        case 's':
                            current_instruction.Append(tile_instruction[instruction_index+1]);

                            switch (tile_instruction[instruction_index+1]) {
                                case 'w':
                                    current_x--;
                                    current_y--;

                                    break;

                                case 'e':
                                    current_y--;

                                    break;
                            }

                            instruction_index++;

                            break;
                    }

                    if (tiles.ContainsKey((current_x, current_y)) == false) {
                        tiles.Add((current_x, current_y), new Tile());
                    }

                    if (instruction_index == tile_instruction.Length - 1) {
                        tiles[(current_x, current_y)].Flip();
                    }
                }
            }

            Console.WriteLine(tiles.Count(tile => tile.Value.color == TileColor.Black));

            // part 2
            var days = 100;

            for (var i = 0; i < days; i++) {
                // determine which neighbor tiles need to be added before evaluating neighbor colors
                var current_tile_indices = new List<(int x, int y)>(tiles.Keys);
                var neighbor_tile_indices = new HashSet<(int x, int y)>();

                foreach (var tile in tiles) {
                    // east => (x + 1, y)
                    neighbor_tile_indices.Add((tile.Key.x + 1, tile.Key.y));

                    // west => (x - 1, y)
                    neighbor_tile_indices.Add((tile.Key.x - 1, tile.Key.y));

                    // northwest => (x, y + 1)
                    neighbor_tile_indices.Add((tile.Key.x, tile.Key.y + 1));

                    // northeast => (x + 1, y + 1)
                    neighbor_tile_indices.Add((tile.Key.x + 1, tile.Key.y + 1));

                    // southwest => (x - 1, y - 1)
                    neighbor_tile_indices.Add((tile.Key.x - 1, tile.Key.y - 1));

                    // southeast => (x, y - 1)
                    neighbor_tile_indices.Add((tile.Key.x, tile.Key.y - 1));
                }

                // add any necessary neighbor tiles so all current tiles have the full set of neighbor tiles present
                foreach (var neighbor_tile_index in neighbor_tile_indices) {
                    if (tiles.ContainsKey(neighbor_tile_index) == false) {
                        tiles.Add(neighbor_tile_index, new Tile());
                    }
                }

                // determine if the conditions for a flip are met for each tile
                foreach (var tile_index in tiles.Keys) {
                    var neighbor_colors = new List<TileColor>();

                    // east => (x + 1, y)
                    if (tiles.ContainsKey((tile_index.x + 1, tile_index.y))) {
                        neighbor_colors.Add(tiles[(tile_index.x + 1, tile_index.y)].color);
                    }
                    
                    // west => (x - 1, y)
                    if (tiles.ContainsKey((tile_index.x - 1, tile_index.y))) {
                        neighbor_colors.Add(tiles[(tile_index.x - 1, tile_index.y)].color);
                    }
                    
                    // northwest => (x, y + 1)
                    if (tiles.ContainsKey((tile_index.x, tile_index.y + 1))) {
                        neighbor_colors.Add(tiles[(tile_index.x, tile_index.y + 1)].color);
                    }
                    
                    // northeast => (x + 1, y + 1)
                    if (tiles.ContainsKey((tile_index.x + 1, tile_index.y + 1))) {
                        neighbor_colors.Add(tiles[(tile_index.x + 1, tile_index.y + 1)].color);
                    }
                    
                    // southwest => (x - 1, y - 1)
                    if (tiles.ContainsKey((tile_index.x - 1, tile_index.y - 1))) {
                        neighbor_colors.Add(tiles[(tile_index.x - 1, tile_index.y - 1)].color);
                    }

                    // southeast => (x, y - 1)
                    if (tiles.ContainsKey((tile_index.x, tile_index.y - 1))) {
                        neighbor_colors.Add(tiles[(tile_index.x, tile_index.y - 1)].color);
                    }

                    //Console.WriteLine($"evaluating conditional flip for tile at index {tile_index}...");
                    tiles[tile_index].EvaluateConditionalFlip(neighbor_colors);
                }

                // run the conditional flip on each tile
                foreach (var tile_index in tiles.Keys) {
                    tiles[tile_index].ConditionalFlip();
                }
            }

            Console.WriteLine(tiles.Count(tile => tile.Value.color == TileColor.Black));
        }
    }
}