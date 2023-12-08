using AdventOfCode.Core;
using System.Text;

namespace AdventOfCode.Solution;

[AoCYearDay(2023, 7)]
public sealed class Day07Year2023 : IAoCDaySolver
{
	[AoCExpectedExampleAnswers("6440")]
	public string SolvePartOne(List<string> input)
	{
		var splitInput = input.Select(i => i.Split(" "));
		List<HandBid> handBids =
		[
			// Using new C# spread operator
			.. splitInput
				.Select(i => 
					new HandBid(i[0], long.Parse(i[1]), isPartOne: true))
				.Order()
		];

		long totalWinnings = 0;

		for (int i = 1; i <= handBids.Count; i++)
		{
			totalWinnings += handBids[i - 1].Bid * i;
		}

		return totalWinnings.ToString();
	}

	[AoCExpectedExampleAnswers("5905")]
	public string SolvePartTwo(List<string> input)
	{
		var splitInput = input.Select(i => i.Split(" "));
		List<HandBid> handBids =
		[
			// Using new C# spread operator
			.. splitInput
				.Select(i => 
					new HandBid(i[0], long.Parse(i[1]), isPartOne: false))
				.Order()
		];

		long totalWinnings = 0;

		for (int i = 1; i <= handBids.Count; i++)
		{
			totalWinnings += handBids[i - 1].Bid * i;
		}

		return totalWinnings.ToString();
	}

	private enum CardHand
	{
		FiveOfKind = 7,
		FourOfKind = 6,
		FullHouse = 5,
		ThreeOfKind = 4,
		TwoPair = 3,
		OnePair = 2,
		HighCard = 1
	}

	private sealed record class HandBid : IComparable
	{
		public string Hand { get; set; }
		public long Bid { get; set; }
		public CardHand CardHand { get; set; }
		public int Rank { get; set; }
		bool IsPartOne { get; set; }

		public HandBid(string hand, long bid, bool isPartOne)
		{
			IsPartOne = isPartOne;
			Hand = hand;
			Bid = bid;
			Rank = SetHandRank();
			CardHand = (CardHand)Enum.Parse(typeof(CardHand), Rank.ToString());
		}

		public int SetHandRank()
		{
			Dictionary<string, int> cardCounts = [];
			string handCopy = Hand;

			int jokerCounts = handCopy.Count(c => c == 'J');
			if (!IsPartOne && jokerCounts == 5)
			{
				return (int)CardHand.FiveOfKind;
			}

			foreach (char card in Hand)
			{
				string cardString = card.ToString();

				if (!IsPartOne && card == 'J')
				{
					continue;
				}

				bool cardHashExists = cardCounts.TryGetValue(cardString, out int cardCount);
				if (!cardHashExists)
				{
					cardCounts.Add(cardString, 1);
				}
				else
				{
					cardCounts[cardString]++;
				}
			}

			if (!IsPartOne && jokerCounts > 0)
			{
				cardCounts = cardCounts
					.OrderByDescending(x => x.Value)
					.ThenByDescending(x => GetCardValue(char.Parse(x.Key), false))
					.ToDictionary();

				bool hasMoreJokers = true;
				while (hasMoreJokers)
				{
					var firstElement = cardCounts.ElementAt(0);

					StringBuilder stringBuilder = new();

					// Inefficient. Who cares!
					bool alreadyReplacedJoker = false;
					for (int i = 0; i < handCopy.Length; i++)
					{
						if (handCopy[i] == 'J'
							&& !alreadyReplacedJoker)
						{
							stringBuilder.Append(firstElement.Key);
							alreadyReplacedJoker = true;
						}
						else
						{
							stringBuilder.Append(handCopy[i]);
						}
					}

					handCopy = stringBuilder.ToString();
					cardCounts[firstElement.Key]++;
					jokerCounts--;

					if (jokerCounts == 0)
					{
						hasMoreJokers = false;
					}
				}
			}

			int fiveOfKinds = cardCounts.Count(c => c.Value == 5);
			int fourOfKinds = cardCounts.Count(c => c.Value == 4);
			int threeOfKinds = cardCounts.Count(c => c.Value == 3);
			int twoOfKinds = cardCounts.Count(c => c.Value == 2);

			int handRank = 0;

			if (fiveOfKinds == 1) handRank = (int)CardHand.FiveOfKind;			// Five of kind
			else if (fourOfKinds == 1) handRank = (int)CardHand.FourOfKind;		// Four of kind
			else if (threeOfKinds == 1
				&& twoOfKinds == 1) handRank = (int)CardHand.FullHouse;         // Full house
			else if (threeOfKinds == 1) handRank = (int)CardHand.ThreeOfKind;	// Three of a kind
			else if (twoOfKinds == 2) handRank = (int)CardHand.TwoPair;			// Two pair
			else if (twoOfKinds == 1) handRank = (int)CardHand.OnePair;			// One pair
			else handRank = (int)CardHand.HighCard;								// High card

			return handRank;
		}

		public int CompareTo(object? obj)
		{
			HandBid secondHand = (HandBid)obj!;

			if (Rank > secondHand.Rank) return 1;
			if (Rank < secondHand.Rank) return -1;
			if (Rank == secondHand.Rank)
			{
				for (int i = 0; i < Hand.Length; i++)
				{
					// Cards are same, but we can't order the entire hand yet
					if (Hand[i] == secondHand.Hand[i])
					{
						continue;
					}

					bool thisHandIsHigherRank =
						GetCardValue(Hand[i], IsPartOne) >
						GetCardValue(secondHand.Hand[i], IsPartOne);
					if (thisHandIsHigherRank)
					{
						return 1;
					}
					else
					{
						return -1;
					}
				}
			}

			// Should never get here, just satisfying compiler
			return 0;
		}

		private static int GetCardValue(char card, bool isPartOne)
		{
			int cardValue = card switch
			{
				'A' => 13,
				'K' => 12,
				'Q' => 11,
				'J' => isPartOne ? 10 : 1,
				'T' => isPartOne ? 9 : 10,
				'9' => isPartOne ? 8 : 9,
				'8' => isPartOne ? 7 : 8,
				'7' => isPartOne ? 6 : 7,
				'6' => isPartOne ? 5 : 6,
				'5' => isPartOne ? 4 : 5,
				'4' => isPartOne ? 3 : 4,
				'3' => isPartOne ? 2 : 3,
				'2' => isPartOne ? 1 : 2,
				_ => 0
			};

			return cardValue;
		}
	}
}
