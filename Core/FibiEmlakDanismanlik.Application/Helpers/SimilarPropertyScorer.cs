using FibiEmlakDanismanlik.Application.Features.Results.CommonPropertyResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Helpers
{
    public static class SimilarPropertyScorer
    {
        public static int Calculate(
            SimilarPropertyReferenceResult reference,
            SimilarPropertyCandidateResult candidate)
        {
            if (reference == null || candidate == null)
                return 0;

            int score = 0;

            score += CalculateLocationScore(reference, candidate);
            score += CalculatePriceScore(reference.Price, candidate.Price);
            score += CalculateSquareMeterScore(reference.SquareMeter, candidate.SquareMeter);

            if (IsHouse(reference.ListingType))
            {
                score += CalculateRoomCountScore(reference.NumberOfRoom, candidate.NumberOfRoom);
            }

            return score;
        }

        private static int CalculateLocationScore(
            SimilarPropertyReferenceResult reference,
            SimilarPropertyCandidateResult candidate)
        {
            if (IsSame(reference.Neighborhood, candidate.Neighborhood))
                return 40;

            if (IsSame(reference.District, candidate.District))
                return 25;

            if (IsSame(reference.City, candidate.City))
                return 10;

            return 0;
        }
        private static decimal CalculateDifferenceRatio(decimal referenceValue, decimal candidateValue)
        {
            if (referenceValue <= 0)
                return 1;

            return Math.Abs(referenceValue - candidateValue) / referenceValue;
        }

private static int CalculatePriceScore(decimal? referencePrice, decimal? candidatePrice)
        {
            if (!referencePrice.HasValue || !candidatePrice.HasValue)
                return 0;

            if (referencePrice.Value <= 0 || candidatePrice.Value <= 0)
                return 0;

            var differenceRatio = CalculateDifferenceRatio(referencePrice.Value, candidatePrice.Value);

            if (differenceRatio <= 0.10m)
                return 25;

            if (differenceRatio <= 0.20m)
                return 18;

            if (differenceRatio <= 0.30m)
                return 10;

            return 0;
        }

        private static int CalculateSquareMeterScore(double? referenceSquareMeter, double? candidateSquareMeter)
        {
            if (!referenceSquareMeter.HasValue || !candidateSquareMeter.HasValue)
                return 0;

            if (referenceSquareMeter.Value <= 0 || candidateSquareMeter.Value <= 0)
                return 0;

            var differenceRatio = CalculateDifferenceRatio(referenceSquareMeter.Value, candidateSquareMeter.Value);

            if (differenceRatio <= 0.10)
                return 20;

            if (differenceRatio <= 0.20)
                return 12;

            if (differenceRatio <= 0.30)
                return 6;

            return 0;
        }
        private static int CalculateRoomCountScore(string? referenceRoomCount, string? candidateRoomCount)
        {
            if (string.IsNullOrWhiteSpace(referenceRoomCount) || string.IsNullOrWhiteSpace(candidateRoomCount))
                return 0;

            if (string.Equals(referenceRoomCount.Trim(), candidateRoomCount.Trim(), StringComparison.OrdinalIgnoreCase))
                return 15;

            return 0;
        }
       

        private static double CalculateDifferenceRatio(double referenceValue, double candidateValue)
        {
            if (referenceValue <= 0)
                return 1;

            return Math.Abs(referenceValue - candidateValue) / referenceValue;
        }


        private static bool IsSame(string? left, string? right)
        {
            if (string.IsNullOrWhiteSpace(left) || string.IsNullOrWhiteSpace(right))
                return false;

            return string.Equals(left.Trim(), right.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsHouse(string? listingType)
        {
            if (string.IsNullOrWhiteSpace(listingType))
                return false;

            return listingType.Trim().Equals("Konut", StringComparison.OrdinalIgnoreCase)
                   || listingType.Trim().Equals("House", StringComparison.OrdinalIgnoreCase);
        }

    }
}
