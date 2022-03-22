using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Система_проверки_знаний {
    public class Question {
        public Question(List<string> possibleAnswers, List<string> correctAnswers) {
            PossibleAnswers = possibleAnswers;
            CorrectAnswers = correctAnswers;
            GivenAnswers = new List<string>();
            Type = QuestionType.UNDEFINED;
            Picture = null;
        }

        public string QuestionText {
            get;
            set;
        }

        public QuestionType Type {
            get;
            set;
        }

        public List<string> PossibleAnswers {
            get;
            set;
        }

        public List<string> CorrectAnswers {
            get;
            set;
        }
        public List<string> GivenAnswers {
            get;
            set;
        }

        public BitmapImage Picture {
            get;
            set;
        }

        public double Score {
            get;
            set;
        }

        public void computeScore() {
            switch (Type) {
            case QuestionType.CHOOSE_ONE:
            case QuestionType.GIVE_ONE: {
                if (GivenAnswers.Count > 0) {
                    Score = CorrectAnswers[0] == GivenAnswers[0] ? 1.0 : 0.0;
                } else {
                    Score = 0.0;
                }
                break;
            }
            case QuestionType.CHOOSE_MULTIPLE: {
                double correctScore = -1.0;
                double incorrectScore = -1.0;
                try {
                    correctScore = (double)(new List<string>(GivenAnswers.Intersect(CorrectAnswers))).Count / (double)CorrectAnswers.Count;
                    incorrectScore = (double)(GivenAnswers.Count - (new List<string>(CorrectAnswers.Intersect(GivenAnswers))).Count) / (double)(PossibleAnswers.Count - CorrectAnswers.Count);
                } catch(Exception) {
                    correctScore = correctScore == -1.0 ? 0.0: correctScore;
                    incorrectScore = incorrectScore == -1.0 ? 0.0 : incorrectScore;
                }
                Score = Math.Max(correctScore - incorrectScore, 0.0);
                break;
            }
            }
        }
    }
    public enum QuestionType {
        CHOOSE_ONE,
        CHOOSE_MULTIPLE,
        GIVE_ONE,
        UNDEFINED
    }
}
