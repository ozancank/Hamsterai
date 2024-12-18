import 'dart:convert';

GetQuestionsResponseModel getQuestionsResponseModelFromJson(String str) =>
    GetQuestionsResponseModel.fromJson(json.decode(str));

class GetQuestionsResponseModel {
  List<Question>? items;
  int? index;
  int? size;
  int? count;
  int? pages;
  bool? hasPrevious;
  bool? hasNext;

  GetQuestionsResponseModel({
    this.items,
    this.index,
    this.size,
    this.count,
    this.pages,
    this.hasPrevious,
    this.hasNext,
  });

  factory GetQuestionsResponseModel.fromJson(Map<String, dynamic> json) =>
      GetQuestionsResponseModel(
        items:
            List<Question>.from(json["items"].map((x) => Question.fromJson(x))),
        index: json["index"],
        size: json["size"],
        count: json["count"],
        pages: json["pages"],
        hasPrevious: json["hasPrevious"],
        hasNext: json["hasNext"],
      );
}

class Question {
  String id;
  int createUser;
  DateTime createDate;
  int lessonId;
  String? questionPictureFileName;
  String? questionPictureExtension;
  String? answerText;
  int status;
  String? lessonName;
  String? ocrMethod;
  int? lessonType;
  bool? manuelSendAgain;
  String? questionText;

  Question({
    required this.id,
    required this.createUser,
    required this.createDate,
    required this.lessonId,
    required this.questionPictureFileName,
    required this.questionPictureExtension,
    required this.answerText,
    required this.status,
    required this.lessonName,
    required this.ocrMethod,
    this.lessonType,
    this.manuelSendAgain,
    this.questionText,
  });

  factory Question.fromJson(Map<String, dynamic> json) => Question(
      id: json["id"],
      createUser: json["createUser"],
      createDate: DateTime.parse(json["createDate"]),
      lessonId: json["lessonId"],
      questionPictureFileName: json["questionPictureFileName"],
      questionPictureExtension: json["questionPictureExtension"],
      answerText: json["answerText"],
      status: json["status"],
      lessonName: json["lessonName"],
      ocrMethod: json["ocrMethod"],
      lessonType: json["lessonType"],
      manuelSendAgain: json["manuelSendAgain"],
      questionText: json["questionText"]);

  Map<String, dynamic> toJson() => {
        "id": id,
        "createUser": createUser,
        "createDate": createDate.toIso8601String(),
        "lessonId": lessonId,
        "questionPictureFileName": questionPictureFileName,
        "questionPictureExtension": questionPictureExtension,
        "answerText": answerText,
        "status": status,
        "lessonName": lessonName,
        "ocrMethod": ocrMethod,
        "lessonType": lessonType,
        "manuelSendAgain": manuelSendAgain,
        "questionText": questionText,
      };
}
