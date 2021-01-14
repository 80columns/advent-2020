using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _22 {
    static class Program {
        static string ToContentString(this Queue<int> queue) {
            var sb = new StringBuilder();

            foreach (var item in queue) {
                sb.Append(item.ToString());
            }

            return sb.ToString();
        }

        static async Task Main(string[] args) {
            var decks = (await File.ReadAllTextAsync("input.txt", Encoding.UTF8)).Split("\n\n");

            // part 1
            var player_1_deck = new Queue<int>(decks[0].Replace("Player 1:", string.Empty).Split("\n", StringSplitOptions.RemoveEmptyEntries).Select(card => Convert.ToInt32(card)));
            var player_2_deck = new Queue<int>(decks[1].Replace("Player 2:", string.Empty).Split("\n", StringSplitOptions.RemoveEmptyEntries).Select(card => Convert.ToInt32(card)));

            while (
                player_1_deck.Any()
             && player_2_deck.Any()
            ) {
                var player_1_top_card = player_1_deck.Dequeue();
                var player_2_top_card = player_2_deck.Dequeue();

                if (player_1_top_card > player_2_top_card) {
                    player_1_deck.Enqueue(player_1_top_card);
                    player_1_deck.Enqueue(player_2_top_card);
                } else if (player_2_top_card > player_1_top_card) {
                    player_2_deck.Enqueue(player_2_top_card);
                    player_2_deck.Enqueue(player_1_top_card);
                }
            }

            var score = 0;
            var winning_deck = player_1_deck.Any() ? player_1_deck : player_2_deck;
            var multiplier = winning_deck.Count;

            foreach (var card in winning_deck) {
                score += card * multiplier;
                multiplier--;
            }

            Console.WriteLine(score);

            // part 2
            player_1_deck = new Queue<int>(decks[0].Replace("Player 1:", string.Empty).Split("\n", StringSplitOptions.RemoveEmptyEntries).Select(card => Convert.ToInt32(card)));
            player_2_deck = new Queue<int>(decks[1].Replace("Player 2:", string.Empty).Split("\n", StringSplitOptions.RemoveEmptyEntries).Select(card => Convert.ToInt32(card)));

            var player_1_top_card_stack = new Stack<int>();
            var player_2_top_card_stack = new Stack<int>();
            var player_1_deck_stack = new Stack<Queue<int>>();
            var player_2_deck_stack = new Stack<Queue<int>>();
            var subgame_index = 0;
            var subgame_index_changed = false;
            var player_1_subgame_hands = new List<HashSet<string>>() { new HashSet<string>() };
            var player_2_subgame_hands = new List<HashSet<string>>() { new HashSet<string>() };

            while (
                player_1_deck.Any()
             && player_2_deck.Any()
            ) {
                subgame_index_changed = false;

                var player_1_hand = player_1_deck.ToContentString();
                var player_2_hand = player_2_deck.ToContentString();

                var player_1_top_card = player_1_deck.Dequeue();
                var player_2_top_card = player_2_deck.Dequeue();

                if (
                    player_1_deck.Count >= player_1_top_card
                 && player_2_deck.Count >= player_2_top_card
                ) {
                    // recursive condition has been met, save top card state + deck state
                    player_1_top_card_stack.Push(player_1_top_card);
                    player_2_top_card_stack.Push(player_2_top_card);

                    player_1_deck_stack.Push(new Queue<int>(player_1_deck));
                    player_2_deck_stack.Push(new Queue<int>(player_2_deck));

                    player_1_deck = new Queue<int>(player_1_deck.Take(player_1_top_card));
                    player_2_deck = new Queue<int>(player_2_deck.Take(player_2_top_card));

                    player_1_subgame_hands[subgame_index].Add(player_1_hand);
                    player_2_subgame_hands[subgame_index].Add(player_2_hand);

                    subgame_index++;
                    subgame_index_changed = true;
                    player_1_subgame_hands.Add(new HashSet<string>());
                    player_2_subgame_hands.Add(new HashSet<string>());
                } else {
                    if (player_1_deck.Any() == false && player_2_top_card > player_1_top_card && player_1_top_card_stack.Any()) {
                        // player 2 wins this sub-game
                        player_1_deck = player_1_deck_stack.Pop();
                        player_2_deck = player_2_deck_stack.Pop();

                        player_2_deck.Enqueue(player_2_top_card_stack.Pop());
                        player_2_deck.Enqueue(player_1_top_card_stack.Pop());

                        player_1_subgame_hands[subgame_index].Add(player_1_hand);
                        player_2_subgame_hands[subgame_index].Add(player_2_hand);

                        subgame_index--;
                        subgame_index_changed = true;
                    } else if (
                        (player_2_deck.Any() == false && player_1_top_card > player_2_top_card && player_1_top_card_stack.Any())
                     || (player_1_subgame_hands[subgame_index].Contains(player_1_hand) && player_2_subgame_hands[subgame_index].Contains(player_2_hand))
                    ) {
                        // playe 1 wins this sub-game
                        player_1_deck = player_1_deck_stack.Pop();
                        player_2_deck = player_2_deck_stack.Pop();

                        player_1_deck.Enqueue(player_1_top_card_stack.Pop());
                        player_1_deck.Enqueue(player_2_top_card_stack.Pop());

                        player_1_subgame_hands[subgame_index].Add(player_1_hand);
                        player_2_subgame_hands[subgame_index].Add(player_2_hand);

                        subgame_index--;
                        subgame_index_changed = true;
                    } else {
                        if (player_2_top_card > player_1_top_card) {
                            player_2_deck.Enqueue(player_2_top_card);
                            player_2_deck.Enqueue(player_1_top_card);
                        } else if (
                            player_1_top_card > player_2_top_card
                        ) {
                            player_1_deck.Enqueue(player_1_top_card);
                            player_1_deck.Enqueue(player_2_top_card);
                        }
                    }
                }

                if (subgame_index_changed == false) {
                    player_1_subgame_hands[subgame_index].Add(player_1_hand);
                    player_2_subgame_hands[subgame_index].Add(player_2_hand);
                }
            }

            score = 0;
            winning_deck = player_1_deck.Any() ? player_1_deck : player_2_deck;
            multiplier = winning_deck.Count;

            foreach (var card in winning_deck) {
                score += card * multiplier;
                multiplier--;
            }

            Console.WriteLine(score);
        }
    }
}