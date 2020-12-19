using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _17 {
    class _3d_cube_container {
        private Dictionary<(int x, int y, int z), cube> cube_dictionary;

        public int min_x { get; private set; }
        public int min_y { get; private set; }
        public int min_z { get; private set; }
        public int max_x { get; private set; }
        public int max_y { get; private set; }
        public int max_z { get; private set; }

        public int active_cube_count => this.cube_dictionary.Where(c => c.Value.state == true).Count();

        public _3d_cube_container() {
            this.cube_dictionary = new Dictionary<(int x, int y, int z), cube>();
        }

        public cube this[int _x, int _y, int _z] {
            get => this.cube_dictionary[(_x, _y, _z)];
            set {
                for (var neighbor_x = _x - 1; neighbor_x <= _x + 1; neighbor_x++) {
                    for (var neighbor_y = _y - 1; neighbor_y <= _y + 1; neighbor_y++) {
                        for (var neighbor_z = _z - 1; neighbor_z <= _z + 1; neighbor_z++) {
                            if (this.cube_dictionary.ContainsKey((neighbor_x, neighbor_y, neighbor_z))) {
                                // if this new cube's neigbor is active, record
                                // the active neighbor
                                if (cube_dictionary[(neighbor_x, neighbor_y, neighbor_z)].state == true) {
                                    value.AddActiveNeighbor();
                                }
                                
                                // if this new cube is active, record an
                                // active neighbor for its neighbor
                                if (value.state == true) {
                                    this.cube_dictionary[(neighbor_x, neighbor_y, neighbor_z)].AddActiveNeighbor();
                                }
                            }
                        }
                    }
                }

                this.cube_dictionary[(_x, _y, _z)] = value;
            }
        }

        public void SetBoundaries(int _min_x, int _min_y, int _min_z, int _max_x, int _max_y, int _max_z) {
            this.min_x = _min_x;
            this.min_y = _min_y;
            this.min_z = _min_z;

            this.max_x = _max_x;
            this.max_y = _max_y;
            this.max_z = _max_z;
        }

        public void Expand() {
            // add a 3d layer around the existing cubes
            this.min_x--;
            this.min_y--;
            this.min_z--;

            this.max_x++;
            this.max_y++;
            this.max_z++;

            // left + right (x)
            for (var y = this.min_y; y <= this.max_y; y++) {
                for (var z = this.min_z; z <= this.max_z; z++) {
                    this[this.min_x, y, z] = new cube();
                    this[this.max_x, y, z] = new cube();
                }
            }

            // bottom + top (y)
            for (var x = this.min_x; x <= this.max_x; x++) {
                for (var z = this.min_z; z <= this.max_z; z++) {
                    this[x, this.min_y, z] = new cube();
                    this[x, this.max_y, z] = new cube();
                }
            }

            // forward + backward (z)
            for (var x = this.min_x; x <= this.max_x; x++) {
                for (var y = this.min_y; y <= this.max_y; y++) {
                    this[x, y, this.min_z] = new cube();
                    this[x, y, this.max_z] = new cube();
                }
            }
        }

        public void Update() {
            // apply transforms to each cube
            for (var x = this.min_x; x <= this.max_x; x++) {
                for (var y = this.min_y; y <= this.max_y; y++) {
                    for (var z = this.min_z; z <= this.max_z; z++) {
                        if (this[x, y, z].Update()) {
                            // update the neighbor count
                            for (var neighbor_x = x - 1; neighbor_x <= x + 1; neighbor_x++) {
                                for (var neighbor_y = y - 1; neighbor_y <= y + 1; neighbor_y++) {
                                    for (var neighbor_z = z - 1; neighbor_z <= z + 1; neighbor_z++) {
                                        if (!(neighbor_x == x && neighbor_y == y && neighbor_z == z) && this.cube_dictionary.ContainsKey((neighbor_x, neighbor_y, neighbor_z))) {
                                            // as this cube was updated to active, update the neighbor
                                            // state for its neighbor cube
                                            if (this[x, y, z].state == true) {
                                                this[neighbor_x, neighbor_y, neighbor_z].AddActiveNeighbor();
                                            } else {
                                                this[neighbor_x, neighbor_y, neighbor_z].RemoveActiveNeighbor();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    class _4d_cube_container {
        private Dictionary<(int x, int y, int z, int w), cube> cube_dictionary;

        public int min_x { get; private set; }
        public int min_y { get; private set; }
        public int min_z { get; private set; }
        public int min_w { get; private set; }
        public int max_x { get; private set; }
        public int max_y { get; private set; }
        public int max_z { get; private set; }
        public int max_w { get; private set; }

        public int active_cube_count => this.cube_dictionary.Where(c => c.Value.state == true).Count();

        public _4d_cube_container() {
            this.cube_dictionary = new Dictionary<(int x, int y, int z, int w), cube>();
        }

        public cube this[int _x, int _y, int _z, int _w] {
            get => this.cube_dictionary[(_x, _y, _z, _w)];
            set {
                for (var neighbor_x = _x - 1; neighbor_x <= _x + 1; neighbor_x++) {
                    for (var neighbor_y = _y - 1; neighbor_y <= _y + 1; neighbor_y++) {
                        for (var neighbor_z = _z - 1; neighbor_z <= _z + 1; neighbor_z++) {
                            for (var neighbor_w = _w - 1; neighbor_w <= _w + 1; neighbor_w++) {
                                if (this.cube_dictionary.ContainsKey((neighbor_x, neighbor_y, neighbor_z, neighbor_w))) {
                                    // if this new cube's neigbor is active, record
                                    // the active neighbor
                                    if (cube_dictionary[(neighbor_x, neighbor_y, neighbor_z, neighbor_w)].state == true) {
                                        value.AddActiveNeighbor();
                                    }
                                    
                                    // if this new cube is active, record an
                                    // active neighbor for its neighbor
                                    if (value.state == true) {
                                        this.cube_dictionary[(neighbor_x, neighbor_y, neighbor_z, neighbor_w)].AddActiveNeighbor();
                                    }
                                }
                            }
                        }
                    }
                }

                this.cube_dictionary[(_x, _y, _z, _w)] = value;
            }
        }

        public void SetBoundaries(int _min_x, int _min_y, int _min_z, int _min_w, int _max_x, int _max_y, int _max_z, int _max_w) {
            this.min_x = _min_x;
            this.min_y = _min_y;
            this.min_z = _min_z;
            this.min_w = _min_w;

            this.max_x = _max_x;
            this.max_y = _max_y;
            this.max_z = _max_z;
            this.max_w = _max_w;
        }

        public void Expand() {
            // add a 3d layer around the existing cubes
            this.min_x--;
            this.min_y--;
            this.min_z--;
            this.min_w--;

            this.max_x++;
            this.max_y++;
            this.max_z++;
            this.max_w++;

            // left + right (x)
            for (var y = this.min_y; y <= this.max_y; y++) {
                for (var z = this.min_z; z <= this.max_z; z++) {
                    for (var w = this.min_w; w <= this.max_w; w++) {
                        this[this.min_x, y, z, w] = new cube();
                        this[this.max_x, y, z, w] = new cube();
                    }
                }
            }

            // bottom + top (y)
            for (var x = this.min_x; x <= this.max_x; x++) {
                for (var z = this.min_z; z <= this.max_z; z++) {
                    for (var w = this.min_w; w <= this.max_w; w++) {
                        this[x, this.min_y, z, w] = new cube();
                        this[x, this.max_y, z, w] = new cube();
                    }
                }
            }

            // forward + backward (z)
            for (var x = this.min_x; x <= this.max_x; x++) {
                for (var y = this.min_y; y <= this.max_y; y++) {
                    for (var w = this.min_w; w <= this.max_w; w++) {
                        this[x, y, this.min_z, w] = new cube();
                        this[x, y, this.max_z, w] = new cube();
                    }
                }
            }

            // forward in time + backward in time (w)
            for (var x = this.min_x; x <= this.max_x; x++) {
                for (var y = this.min_y; y <= this.max_y; y++) {
                    for (var z = this.min_z; z <= this.max_z; z++) {
                        this[x, y, z, this.min_w] = new cube();
                        this[x, y, z, this.max_w] = new cube();
                    }
                }
            }
        }

        public void Update() {
            // apply transforms to each cube
            for (var x = this.min_x; x <= this.max_x; x++) {
                for (var y = this.min_y; y <= this.max_y; y++) {
                    for (var z = this.min_z; z <= this.max_z; z++) {
                        for (var w = this.min_w; w <= this.max_w; w++) {
                            if (this[x, y, z, w].Update()) {
                                // update the neighbor count
                                for (var neighbor_x = x - 1; neighbor_x <= x + 1; neighbor_x++) {
                                    for (var neighbor_y = y - 1; neighbor_y <= y + 1; neighbor_y++) {
                                        for (var neighbor_z = z - 1; neighbor_z <= z + 1; neighbor_z++) {
                                            for (var neighbor_w = w - 1; neighbor_w <= w + 1; neighbor_w++) {
                                                if (!(neighbor_x == x && neighbor_y == y && neighbor_z == z && neighbor_w == w) && this.cube_dictionary.ContainsKey((neighbor_x, neighbor_y, neighbor_z, neighbor_w))) {
                                                    // as this cube was updated to active, update the neighbor
                                                    // state for its neighbor cube
                                                    if (this[x, y, z, w].state == true) {
                                                        this[neighbor_x, neighbor_y, neighbor_z, neighbor_w].AddActiveNeighbor();
                                                    } else {
                                                        this[neighbor_x, neighbor_y, neighbor_z, neighbor_w].RemoveActiveNeighbor();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    class cube {
        private bool current_state;
        private bool? transformed_state;
        private int active_neighbor_count;

        public bool state => this.current_state;
        public int active_neighbors => this.active_neighbor_count;

        public cube(bool _current_state = false) {
            this.current_state = _current_state;
            this.transformed_state = null;
            this.active_neighbor_count = 0;
        }

        public void Transform(bool _transformed_state) {
            this.transformed_state = _transformed_state;
        }

        public void AddActiveNeighbor() {
            this.active_neighbor_count++;
        }

        public void RemoveActiveNeighbor() {
            if (this.active_neighbor_count > 0) {
                this.active_neighbor_count--;
            }
        }

        public bool Update() {
            if (this.transformed_state.HasValue) {
                this.current_state = this.transformed_state.Value;
                this.transformed_state = null;

                return true;
            }

            return false;
        }
    }

    class Program {
        static async Task Main(string[] args) {
            var input_grid = await File.ReadAllLinesAsync("input.txt", Encoding.UTF8);
            var _3d_container = new _3d_cube_container();

            Process3dCycles(input_grid, _3d_container, 6);

            Console.WriteLine(_3d_container.active_cube_count);

            var _4d_container = new _4d_cube_container();

            Process4dCycles(input_grid, _4d_container, 6);

            Console.WriteLine(_4d_container.active_cube_count);
        }

        static void Process3dCycles(string[] _input_grid, _3d_cube_container _container, int _cycles) {
            _container.SetBoundaries(0, 0, 0, _input_grid.Length - 1, _input_grid[0].Length - 1, 0);

            // initialize container from the input grid
            for (var x = 0; x < _input_grid.Length; x++) {
                for (var y = 0; y < _input_grid[x].Length; y++) {
                    _container[x, y, 0] = new cube(_input_grid[x][y] == '#');
                }
            }
            
            for (var cycle = 0; cycle < _cycles; cycle++) {
                // add a 3d layer around the existing cubes
                _container.Expand();

                // transform the cubes according to these rules:
                // 1. if a cube is active and exactly 2 or 3 of its neighbors are also active, the cube remains active. Otherwise, the cube becomes inactive.
                //
                // 2. if a cube is inactive but exactly 3 of its neighbors are active, the cube becomes active. Otherwise, the cube remains inactive.
                for (var x = _container.min_x; x <= _container.max_x; x++) {
                    for (var y = _container.min_y; y <= _container.max_y; y++) {
                        for (var z = _container.min_z; z <= _container.max_z; z++) {
                            if (_container[x, y, z].state == true) {
                                if (_container[x, y, z].active_neighbors != 2 && _container[x, y, z].active_neighbors != 3) {
                                    _container[x, y, z].Transform(false);
                                }
                            } else if (_container[x, y, z].active_neighbors == 3) {
                                _container[x, y, z].Transform(true);
                            }
                        }
                    }
                }

                // apply transforms to each cube
                _container.Update();
            }
        }

        static void Process4dCycles(string[] _input_grid, _4d_cube_container _container, int _cycles) {
            _container.SetBoundaries(0, 0, 0, 0, _input_grid.Length - 1, _input_grid[0].Length - 1, 0, 0);

            for (var x = 0; x < _input_grid.Length; x++) {
                for (var y = 0; y < _input_grid[x].Length; y++) {
                    _container[x, y, 0, 0] = new cube(_input_grid[x][y] == '#' ? true : false);
                }
            }

            for (var cycle = 0; cycle < _cycles; cycle++) {
                // add a 4d layer around the existing hypercubes
                _container.Expand();

                // transform the cubes according to these rules:
                // 1. if a cube is active and exactly 2 or 3 of its neighbors are also active, the cube remains active. Otherwise, the cube becomes inactive.
                //
                // 2. if a cube is inactive but exactly 3 of its neighbors are active, the cube becomes active. Otherwise, the cube remains inactive.
                for (var x = _container.min_x; x <= _container.max_x; x++) {
                    for (var y = _container.min_y; y <= _container.max_y; y++) {
                        for (var z = _container.min_z; z <= _container.max_z; z++) {
                            for (var w = _container.min_w; w <= _container.max_w; w++) {
                                if (_container[x, y, z, w].state == true) {
                                    if (_container[x, y, z, w].active_neighbors != 2 && _container[x, y, z, w].active_neighbors != 3) {
                                        _container[x, y, z, w].Transform(false);
                                    }
                                } else if (_container[x, y, z, w].active_neighbors == 3) {
                                    _container[x, y, z, w].Transform(true);
                                }
                            }
                        }
                    }
                }

                // apply transforms to each cube
                _container.Update();
            }
        }
    }
}