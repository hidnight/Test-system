using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Система_проверки_знаний {
    public static class TestFactory {
        public static Test CreateTest(string[] fileLines) {
            Test test = new Test();
            string[] time = fileLines[0].Split(':');
            test.Minutes = int.Parse(time[0]);
            test.Seconds = int.Parse(time[1]);
            if (test.Minutes < 0 || test.Minutes > 59 ||
                test.Seconds < 0 || test.Seconds > 59 ||
                (test.Minutes <= 0 &&
                test.Seconds <= 0)) {
                throw new Exception("Файл имеет неверное содержание, ошибка в строке " + 0);
            }
            State state = State.NULL;
            Question question = new Question(new List<string>(), new List<string>());
            for (int i = 1; i < fileLines.Length; i++) {
                if (fileLines[i].Trim() == "\n")
                    continue;
                switch (fileLines[i].Trim()) {
                case "Q": {
                    if (state != State.A && state != State.NULL) {
                        throw new Exception("Файл имеет неверное содержание, ошибка в строке " + i);
                    }
                    if (state != State.NULL) {
                        if (question.CorrectAnswers.Count > 1) {
                            question.Type = QuestionType.CHOOSE_MULTIPLE;
                        } else {
                            if (question.PossibleAnswers.Count > 0) {
                                question.Type = QuestionType.CHOOSE_ONE;
                            } else {
                                question.Type = QuestionType.GIVE_ONE;
                            }
                        }
                        test.Questions.Add(question);
                    }
                    state = State.Q;

                    // БП-2.2
                    if (question.Type != QuestionType.GIVE_ONE && (new List<string>(question.CorrectAnswers.Intersect(question.PossibleAnswers))).Count != question.CorrectAnswers.Count) {
                        throw new Exception("Файл имеет неверное содержание, ошибка в вопросе " + (test.Questions.IndexOf(question) + 1));
                    }
                    // БП-2.4
                    if (question.Type == QuestionType.GIVE_ONE && question.CorrectAnswers.Count > 1) {
                        throw new Exception("Файл имеет неверное содержание, ошибка в вопросе " + (test.Questions.IndexOf(question) + 1));
                    }

                    question = new Question(new List<string>(), new List<string>());
                    break;
                }
                case "P": {
                    if (state != State.Q) {
                        throw new Exception("Файл имеет неверное содержание, ошибка в строке " + i);
                    }
                    state = State.P;
                    break;
                }
                case "V": {
                    if (state != State.P) {
                        throw new Exception("Файл имеет неверное содержание, ошибка в строке " + i);
                    }
                    state = State.V;
                    break;
                }
                case "A": {
                    if (state != State.V) {
                        throw new Exception("Файл имеет неверное содержание, ошибка в строке " + i);
                    }
                    state = State.A;
                    break;
                }
                default: {
                    switch (state) {
                    case (State.Q): {
                        question.QuestionText += fileLines[i].Trim();
                        break;
                    }
                    case (State.P): {
                        question.Picture = new System.Windows.Media.Imaging.BitmapImage(new Uri(fileLines[i].Trim()));
                        break;
                    }
                    case (State.V): {
                        question.PossibleAnswers.Add(fileLines[i].Trim());
                        break;
                    }
                    case (State.A): {
                        question.CorrectAnswers.Add(fileLines[i].Trim());
                        break;
                    }
                    default: {
                        throw new Exception("Файл имеет неверное содержание, ошибка в строке " + i);
                    }
                    }
                    break;
                }
                }
            }
            if (question.CorrectAnswers.Count > 1) {
                question.Type = QuestionType.CHOOSE_MULTIPLE;
            } else {
                if (question.PossibleAnswers.Count > 0) {
                    question.Type = QuestionType.CHOOSE_ONE;
                } else {
                    question.Type = QuestionType.GIVE_ONE;
                }
            }
            test.Questions.Add(question);
            // БП-2.2
            if (question.Type != QuestionType.GIVE_ONE && (new List<string>(question.CorrectAnswers.Intersect(question.PossibleAnswers))).Count != question.CorrectAnswers.Count) {
                throw new Exception("Файл имеет неверное содержание, ошибка в вопросе " + (test.Questions.IndexOf(question) + 1));
            }
            // БП-2.4
            if (question.Type == QuestionType.GIVE_ONE && question.CorrectAnswers.Count > 1) {
                throw new Exception("Файл имеет неверное содержание, ошибка в вопросе " + (test.Questions.IndexOf(question) + 1));
            }
            return test;
        }
        enum State {
            Q,
            P,
            V,
            A,
            NULL
        }
    }
    public class Test {
        public Test() {
            Questions = new List<Question>();
            Minutes = 0;
            Seconds = 0;
        }

        public List<Question> Questions {
            get;
            set;
        }

        public int Minutes {
            get;
            set;
        }

        public int Seconds {
            get;
            set;
        }
    }
}
