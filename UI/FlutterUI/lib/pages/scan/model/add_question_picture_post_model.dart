import 'dart:convert';

AddQuestionPicturePostModel addQuestionModelFromJson(String str) =>
    AddQuestionPicturePostModel.fromJson(json.decode(str));

String addQuestionModelToJson(AddQuestionPicturePostModel data) =>
    json.encode(data.toJson());

class AddQuestionPicturePostModel {
  int lessonId;
  String questionPictureBase64;
  String? questionSmallPictureBase64;
  String questionPictureFileName;
  bool isVisual;

  AddQuestionPicturePostModel({
    required this.lessonId,
    required this.questionPictureBase64,
    required this.questionPictureFileName,
    this.questionSmallPictureBase64,
    required this.isVisual,
  });

  factory AddQuestionPicturePostModel.fromJson(Map<String, dynamic> json) =>
      AddQuestionPicturePostModel(
        lessonId: json["lessonId"],
        questionPictureBase64: json["questionPictureBase64"],
        questionSmallPictureBase64: json["questionSmallPictureBase64"],
        questionPictureFileName: json["questionPictureFileName"],
        isVisual: json["isVisual"]
      );

  Map<String, dynamic> toJson() => {
        "lessonId": lessonId,
        "questionPictureBase64": questionPictureBase64,
        "questionPictureFileName": questionPictureFileName,
        "questionSmallPictureBase64": questionSmallPictureBase64,
        "isVisual": isVisual,
      };
}
