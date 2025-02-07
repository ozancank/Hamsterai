import 'dart:convert';

QuizModel quizModelFromJson(String str) => QuizModel.fromJson(json.decode(str));

String quizModelToJson(QuizModel data) => json.encode(data.toJson());

class QuizModel {
  String id;
  int userId;
  int lessonId;
  int timeSecond;
  int status;
  int correctCount;
  int wrongCount;
  int emptyCount;
  int successRate;
  List<String> gainNames;
  List<QuizQuestion> questions;
  Map<String, String?> rightOptions;
  Map<String, String?> answers;
  String userFullName;
  dynamic schoolName;
  String lessonName;

  QuizModel({
    required this.id,
    required this.userId,
    required this.lessonId,
    required this.timeSecond,
    required this.status,
    required this.correctCount,
    required this.wrongCount,
    required this.emptyCount,
    required this.successRate,
    required this.gainNames,
    required this.questions,
    required this.rightOptions,
    required this.answers,
    required this.userFullName,
    required this.schoolName,
    required this.lessonName,
  });

  factory QuizModel.fromJson(Map<String, dynamic> json) => QuizModel(
        id: json["id"],
        userId: json["userId"],
        lessonId: json["lessonId"],
        timeSecond: json["timeSecond"],
        status: json["status"],
        correctCount: json["correctCount"],
        wrongCount: json["wrongCount"],
        emptyCount: json["emptyCount"],
        successRate: json["successRate"],
        gainNames: List<String>.from(json["gainNames"].map((x) => x)),
        questions: List<QuizQuestion>.from(
            json["questions"].map((x) => QuizQuestion.fromJson(x))),
        rightOptions: Map.from(json["rightOptions"])
            .map((k, v) => MapEntry<String, String?>(k, v)),
        answers: Map.from(json["answers"])
            .map((k, v) => MapEntry<String, String?>(k, v)),
        userFullName: json["userFullName"],
        schoolName: json["schoolName"],
        lessonName: json["lessonName"],
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "userId": userId,
        "lessonId": lessonId,
        "timeSecond": timeSecond,
        "status": status,
        "correctCount": correctCount,
        "wrongCount": wrongCount,
        "emptyCount": emptyCount,
        "successRate": successRate,
        "gainNames": List<dynamic>.from(gainNames.map((x) => x)),
        "questions": List<dynamic>.from(questions.map((x) => x.toJson())),
        "rightOptions": Map.from(rightOptions)
            .map((k, v) => MapEntry<String, dynamic>(k, v)),
        "answers":
            Map.from(answers).map((k, v) => MapEntry<String, dynamic>(k, v)),
        "userFullName": userFullName,
        "schoolName": schoolName,
        "lessonName": lessonName,
      };
}

class QuizQuestion {
  String id;
  int sortNo;
  String quizId;
  String question;
  String questionPictureFileName;
  String questionPictureExtension;
  String answer;
  String answerPictureFileName;
  String answerPictureExtension;
  String rightOption;
  dynamic answerOption;
  int optionCount;
  int gainId;
  String gainName;

  QuizQuestion({
    required this.id,
    required this.sortNo,
    required this.quizId,
    required this.question,
    required this.questionPictureFileName,
    required this.questionPictureExtension,
    required this.answer,
    required this.answerPictureFileName,
    required this.answerPictureExtension,
    required this.rightOption,
    required this.answerOption,
    required this.optionCount,
    required this.gainId,
    required this.gainName,
  });

  factory QuizQuestion.fromJson(Map<String, dynamic> json) => QuizQuestion(
        id: json["id"],
        sortNo: json["sortNo"],
        quizId: json["quizId"],
        question: json["question"],
        questionPictureFileName: json["questionPictureFileName"],
        questionPictureExtension: json["questionPictureExtension"],
        answer: json["answer"],
        answerPictureFileName: json["answerPictureFileName"],
        answerPictureExtension: json["answerPictureExtension"],
        rightOption: json["rightOption"],
        answerOption: json["answerOption"],
        optionCount: json["optionCount"],
        gainId: json["gainId"],
        gainName: json["gainName"],
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "sortNo": sortNo,
        "quizId": quizId,
        "question": question,
        "questionPictureFileName": questionPictureFileName,
        "questionPictureExtension": questionPictureExtension,
        "answer": answer,
        "answerPictureFileName": answerPictureFileName,
        "answerPictureExtension": answerPictureExtension,
        "rightOption": rightOption,
        "answerOption": answerOption,
        "optionCount": optionCount,
        "gainId": gainId,
        "gainName": gainName,
      };
}
