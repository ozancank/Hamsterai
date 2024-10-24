import 'dart:convert';

UpdateQuizPostModel updateQuizPostModelFromJson(String str) =>
    UpdateQuizPostModel.fromJson(json.decode(str));

String updateQuizPostModelToJson(UpdateQuizPostModel data) =>
    json.encode(data.toJson());

class UpdateQuizPostModel {
  String quizId;
  int timeSecond;
  int status;
  List<Answer> answers;

  UpdateQuizPostModel({
    required this.quizId,
    required this.timeSecond,
    required this.status,
    required this.answers,
  });

  factory UpdateQuizPostModel.fromJson(Map<String, dynamic> json) =>
      UpdateQuizPostModel(
        quizId: json["quizId"],
        timeSecond: json["timeSecond"],
        status: json["status"],
        answers:
            List<Answer>.from(json["answers"].map((x) => Answer.fromJson(x))),
      );

  Map<String, dynamic> toJson() => {
        "quizId": quizId,
        "timeSecond": timeSecond,
        "status": status,
        "answers": List<dynamic>.from(answers.map((x) => x.toJson())),
      };
}

class Answer {
  String questionId;
  String answerOption;

  Answer({
    required this.questionId,
    required this.answerOption,
  });

  factory Answer.fromJson(Map<String, dynamic> json) => Answer(
        questionId: json["questionId"],
        answerOption: json["answerOption"],
      );

  Map<String, dynamic> toJson() => {
        "questionId": questionId,
        "answerOption": answerOption,
      };
}
