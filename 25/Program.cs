using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace _25 {
    class Program {
        static async Task Main(string[] args) {
            var public_keys = await File.ReadAllLinesAsync("input.txt", Encoding.UTF8);
            var card_public_key = Convert.ToInt64(public_keys[0]);
            var door_public_key = Convert.ToInt64(public_keys[1]);

            var subject_number = (long)7;
            var loop_size = (long)0;
            var value = (long)1;

            while (value != card_public_key) {
                value *= subject_number;
                value = value % 20_201_227;

                loop_size++;
            }

            subject_number = door_public_key;
            value = 1;

            for (var i = 0; i < loop_size; i++) {
                value *= subject_number;
                value = value % 20_201_227;
            }

            Console.WriteLine(value);
        }
    }
}