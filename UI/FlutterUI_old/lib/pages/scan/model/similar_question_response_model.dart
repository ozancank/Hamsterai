import 'dart:convert';

SimilarQuestionResponseModel similarQuestionResponseModelFromJson(String str) =>
    SimilarQuestionResponseModel.fromJson(json.decode(str));

String similarQuestionResponseModelToJson(SimilarQuestionResponseModel data) =>
    json.encode(data.toJson());

class SimilarQuestionResponseModel {
  String? id;
  int? createUser;
  DateTime? createDate;
  int? lessonId;
  String? questionPictureFileName;
  String? questionPictureExtension;
  String? responseQuestion;
  String? responseQuestionFileName;
  String? responseQuestionExtension;
  String? responseAnswer;
  String? responseAnswerFileName;
  String? responseAnswerExtension;
  int? status;
  String? lessonName;

  SimilarQuestionResponseModel({
    this.id,
    this.createUser,
    this.createDate,
    this.lessonId,
    this.questionPictureFileName,
    this.questionPictureExtension,
    this.responseQuestion,
    this.responseQuestionFileName,
    this.responseQuestionExtension,
    this.responseAnswer,
    this.responseAnswerFileName,
    this.responseAnswerExtension,
    this.status,
    this.lessonName,
  });

  factory SimilarQuestionResponseModel.fromJson(Map<String, dynamic> json) =>
      SimilarQuestionResponseModel(
        id: json["id"],
        createUser: json["createUser"],
        createDate: DateTime.parse(json["createDate"]),
        lessonId: json["lessonId"],
        questionPictureFileName: json["questionPictureFileName"],
        questionPictureExtension: json["questionPictureExtension"],
        responseQuestion: json["responseQuestion"],
        responseQuestionFileName: json["responseQuestionFileName"],
        responseQuestionExtension: json["responseQuestionExtension"],
        responseAnswer: json["responseAnswer"],
        responseAnswerFileName: json["responseAnswerFileName"],
        responseAnswerExtension: json["responseAnswerExtension"],
        status: json["status"],
        lessonName: json["lessonName"],
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "createUser": createUser,
        "createDate": createDate?.toIso8601String(),
        "lessonId": lessonId,
        "questionPictureFileName": questionPictureFileName,
        "questionPictureExtension": questionPictureExtension,
        "responseQuestion": responseQuestion,
        "responseQuestionFileName": responseQuestionFileName,
        "responseQuestionExtension": responseQuestionExtension,
        "responseAnswer": responseAnswer,
        "responseAnswerFileName": responseAnswerFileName,
        "responseAnswerExtension": responseAnswerExtension,
        "status": status,
        "lessonName": lessonName,
      };
}
