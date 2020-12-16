using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace _12 {
    class Program {
        static async Task Main(string[] args) {
            var nav_instructions = await File.ReadAllLinesAsync("input.txt", Encoding.UTF8);

            /*
             * Part 1
             *
             * Find the Manhattan distance between the ship and its starting point
             */
            var north = 0;
            var west = 0;
            var direction = 3; // 0 = north, 1 = west, 2 = south, 3 = east

            for (var i = 0; i < nav_instructions.Length; i++) {
                switch (nav_instructions[i][0]) {
                    case 'N':
                        north += Convert.ToInt32(nav_instructions[i].Substring(1));
                        break;

                    case 'W':
                        west += Convert.ToInt32(nav_instructions[i].Substring(1));
                        break;

                    case 'S':
                        north -= Convert.ToInt32(nav_instructions[i].Substring(1));
                        break;

                    case 'E':
                        west -= Convert.ToInt32(nav_instructions[i].Substring(1));
                        break;

                    case 'R':
                        direction -= Convert.ToInt32(nav_instructions[i].Substring(1)) / 90;
                        direction = (direction < 0) ? direction + 4 : direction;
                        break;

                    case 'L':
                        direction += Convert.ToInt32(nav_instructions[i].Substring(1)) / 90;
                        direction = (direction > 3) ? direction - 4 : direction;
                        break;

                    case 'F':
                        switch (direction) {
                            case 0:
                                north += Convert.ToInt32(nav_instructions[i].Substring(1));
                                break;
                            case 1:
                                west += Convert.ToInt32(nav_instructions[i].Substring(1));
                                break;
                            case 2:
                                north -= Convert.ToInt32(nav_instructions[i].Substring(1));
                                break;
                            case 3:
                                west -= Convert.ToInt32(nav_instructions[i].Substring(1));
                                break;
                        }

                        break;
                }
            }

            Console.WriteLine(Math.Abs(north) + Math.Abs(west));

            /*
             * Part 2
             *
             * Find the Manhattan distance between the ship and its starting point
             * using a waypoint
             */
            var temp_distance = -1;
            var waypoint_lat = 1;
            var waypoint_long = 10;
            var waypoint_lat_direction = 0; // 0 = north, 1 = south
            var waypoint_long_direction = 1; // 0 = west, 1 = east

            north = 0;
            west = 0;

            for (var i = 0; i < nav_instructions.Length; i++) {
                switch (nav_instructions[i][0]) {
                    case 'N':
                        waypoint_lat += (waypoint_lat_direction == 0) ? Convert.ToInt32(nav_instructions[i].Substring(1)) : -1 * Convert.ToInt32(nav_instructions[i].Substring(1));

                        if (waypoint_lat < 0) {
                            waypoint_lat_direction = (waypoint_lat_direction == 0) ? 1 : 0;
                            waypoint_lat *= -1;
                        }

                        break;
                    
                    case 'S':
                        waypoint_lat += (waypoint_lat_direction == 1) ? Convert.ToInt32(nav_instructions[i].Substring(1)) : -1 * Convert.ToInt32(nav_instructions[i].Substring(1));

                        if (waypoint_lat < 0) {
                            waypoint_lat_direction = (waypoint_lat_direction == 1) ? 0 : 1;
                            waypoint_lat *= -1;
                        }

                        break;

                    case 'E':
                        waypoint_long += (waypoint_long_direction == 1) ? Convert.ToInt32(nav_instructions[i].Substring(1)) : -1 * Convert.ToInt32(nav_instructions[i].Substring(1));

                        if (waypoint_long < 0) {
                            waypoint_long_direction = (waypoint_long_direction == 1) ? 0 : 1;
                            waypoint_long *= -1;
                        }

                        break;

                    case 'W':
                        waypoint_long += (waypoint_long_direction == 0) ? Convert.ToInt32(nav_instructions[i].Substring(1)) : -1 * Convert.ToInt32(nav_instructions[i].Substring(1));

                        if (waypoint_long < 0) {
                            waypoint_long_direction = (waypoint_long_direction == 0) ? 1 : 0;
                            waypoint_long *= -1;
                        }

                        break;

                    case 'L':
                        var left_rotations = Convert.ToInt32(nav_instructions[i].Substring(1)) / 90;

                        for (var j = 0; j < left_rotations; j++) {
                            if (waypoint_lat_direction == 0 && waypoint_long_direction == 0) {
                                // northwest to southwest
                                waypoint_lat_direction = 1;
                                
                                temp_distance = waypoint_lat;
                                waypoint_lat = waypoint_long;
                                waypoint_long = temp_distance;
                            } else if (waypoint_lat_direction == 1 && waypoint_long_direction == 0) {
                                // southwest to southeast
                                waypoint_long_direction = 1;

                                temp_distance = waypoint_lat;
                                waypoint_lat = waypoint_long;
                                waypoint_long = temp_distance;
                            } else if (waypoint_lat_direction == 1 && waypoint_long_direction == 1) {
                                // southeast to northeast
                                waypoint_lat_direction = 0;

                                temp_distance = waypoint_lat;
                                waypoint_lat = waypoint_long;
                                waypoint_long = temp_distance;
                            } else if (waypoint_lat_direction == 0 && waypoint_long_direction == 1) {
                                // northeast to northwest
                                waypoint_long_direction = 0;

                                temp_distance = waypoint_lat;
                                waypoint_lat = waypoint_long;
                                waypoint_long = temp_distance;
                            }
                        }

                        break;

                    case 'R':
                        var right_rotations = Convert.ToInt32(nav_instructions[i].Substring(1)) / 90;

                        for (var j = 0; j < right_rotations; j++) {
                            if (waypoint_lat_direction == 0 && waypoint_long_direction == 0) {
                                // northwest to northeast
                                waypoint_long_direction = 1;

                                temp_distance = waypoint_lat;
                                waypoint_lat = waypoint_long;
                                waypoint_long = temp_distance;
                            } else if (waypoint_lat_direction == 0 && waypoint_long_direction == 1) {
                                // northeast to southeast
                                waypoint_lat_direction = 1;

                                temp_distance = waypoint_lat;
                                waypoint_lat = waypoint_long;
                                waypoint_long = temp_distance;
                            } else if (waypoint_lat_direction == 1 && waypoint_long_direction == 1) {
                                // southeast to southwest
                                waypoint_long_direction = 0;

                                temp_distance = waypoint_lat;
                                waypoint_lat = waypoint_long;
                                waypoint_long = temp_distance;
                            } else if (waypoint_lat_direction == 1 && waypoint_long_direction == 0) {
                                // southwest to northwest
                                waypoint_lat_direction = 0;

                                temp_distance = waypoint_lat;
                                waypoint_lat = waypoint_long;
                                waypoint_long = temp_distance;
                            } 
                        }

                        break;

                    case 'F':
                        var waypoint_multiplier = Convert.ToInt32(nav_instructions[i].Substring(1));

                        north += ((waypoint_lat_direction == 0) ? 1 : -1) * waypoint_lat * waypoint_multiplier;
                        west += ((waypoint_long_direction == 0) ? 1 : -1) * waypoint_long * waypoint_multiplier;

                        break;
                }
            }

            Console.WriteLine(Math.Abs(north) + Math.Abs(west));
        }
    }
}