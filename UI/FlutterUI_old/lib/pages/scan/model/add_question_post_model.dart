import 'dart:convert';

AddQuestionPostModel addQuestionModelFromJson(String str) =>
    AddQuestionPostModel.fromJson(json.decode(str));

String addQuestionModelToJson(AddQuestionPostModel data) =>
    json.encode(data.toJson());

class AddQuestionPostModel {
  int lessonId;
  String questionPictureBase64;
  String questionPictureFileName;

  AddQuestionPostModel({
    required this.lessonId,
    required this.questionPictureBase64,
    required this.questionPictureFileName,
  });

  factory AddQuestionPostModel.fromJson(Map<String, dynamic> json) =>
      AddQuestionPostModel(
        lessonId: json["lessonId"],
        questionPictureBase64: json["questionPictureBase64"],
        questionPictureFileName: json["questionPictureFileName"],
      );

  Map<String, dynamic> toJson() => {
        "lessonId": lessonId,
        "questionPictureBase64": questionPictureBase64,
        "questionPictureFileName": questionPictureFileName,
      };
}
