using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Drawing_Mistakes_Detection
{
    /// <summary>
    /// A class representing a prediction of Custom Vision API.
    /// </summary>
    class PredictionResult
    {
        private float PROBABILITY_THRESHOLD = 0.5f;

        public string Id { get; set; }
        public string Project { get; set; }
        public string Iteration { get; set; }
        public Prediction[] Predictions { get; set; }

        /// <summary>
        /// Returns an array of tag names with the highest probability predictions.
        /// </summary>
        public string[] GetBestPredictions()
        {
            List<string> bestPredictionTags = new List<string>();
            foreach(Prediction prediction in Predictions) {
                if(prediction.Probability >= PROBABILITY_THRESHOLD)
                {
                    bestPredictionTags.Add(prediction.Tag);
                }
            }

            return bestPredictionTags.ToArray<string>();
        }

        public class Prediction
        {
            public string TagId { get; set; }
            public string Tag { get; set; }
            public float Probability { get; set; }
        }
    }
}
