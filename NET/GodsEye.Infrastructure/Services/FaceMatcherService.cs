
using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces;
using System.Numerics.Tensors;

namespace GodsEye.Infrastructure.Services
{
    public class FaceMatcherService : IFaceMatcherService
    {
        public (int, float) FindMatch(float[] extractedVector, List<PersonCache> persons, float threshold = 0.65f)
        {
            float bestScore = -1f;
            int matchedId = 0;

            //// Comparação em alta velocidade usando SIMD do .NET
            foreach (var person in persons)
            {
                //Calcula a Similaridade de Cosseno(1.0 = igual, 0.0 = diferente)
                float score = TensorPrimitives.CosineSimilarity(person.Embedding, extractedVector);

                if (score > bestScore)
                {
                    bestScore = score;
                    matchedId = person.Id;
                }
            }

            return bestScore >= threshold ? (matchedId, bestScore) : (0, bestScore);
        }
    }
}
