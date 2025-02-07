import 'dart:convert';

AddQuestionResponseModel addQuestionResponseModelFromJson(String str) =>
    AddQuestionResponseModel.fromJson(json.decode(str));

String addQuestionResponseModelToJson(AddQuestionResponseModel data) =>
    json.encode(data.toJson());

class AddQuestionResponseModel {
  String? id;
  int? createUser;
  DateTime? createDate;
  int? lessonId;
  String? questionPictureFileName;
  String? questionPictureExtension;
  String? answerText;
  int? status;
  String? lessonName;

  AddQuestionResponseModel({
    this.id,
    this.createUser,
    this.createDate,
    this.lessonId,
    this.questionPictureFileName,
    this.questionPictureExtension,
    this.answerText,
    this.status,
    this.lessonName,
  });

  factory AddQuestionResponseModel.fromJson(Map<String, dynamic> json) =>
      AddQuestionResponseModel(
        id: json["id"],
        createUser: json["createUser"],
        createDate: DateTime.parse(json["createDate"]),
        lessonId: json["lessonId"],
        questionPictureFileName: json["questionPictureFileName"],
        questionPictureExtension: json["questionPictureExtension"],
        answerText: json["answerText"],
        status: json["status"],
        lessonName: json["lessonName"],
      );

  Map<String, dynamic> toJson() => {
        "id": id ?? "",
        "createUser": createUser ?? "",
        "createDate": createDate!.toIso8601String(),
        "lessonId": lessonId ?? 0,
        "questionPictureFileName": questionPictureFileName ?? '',
        "questionPictureExtension": questionPictureExtension ?? '',
        "answerText": answerText ?? '',
        "status": status ?? 0,
        "lessonName": lessonName ?? '',
      };
}
