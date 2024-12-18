import 'dart:convert';

GetQuestionsResponseModel getQuestionsResponseModelFromJson(String str) =>
    GetQuestionsResponseModel.fromJson(json.decode(str));

String getQuestionsResponseModelToJson(GetQuestionsResponseModel data) =>
    json.encode(data.toJson());

class GetQuestionsResponseModel {
  List<Question> items;
  int index;
  int size;
  int count;
  int pages;
  bool hasPrevious;
  bool hasNext;

  GetQuestionsResponseModel({
    required this.items,
    required this.index,
    required this.size,
    required this.count,
    required this.pages,
    required this.hasPrevious,
    required this.hasNext,
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

  Map<String, dynamic> toJson() => {
        "items": List<Question>.from(items.map((x) => x.toJson())),
        "index": index,
        "size": size,
        "count": count,
        "pages": pages,
        "hasPrevious": hasPrevious,
        "hasNext": hasNext,
      };
}

class Question {
  String id;
  int createUser;
  DateTime createDate;
  int lessonId;
  String questionPictureFileName;
  String questionPictureExtension;
  String answerText;
  int status;
  String lessonName;

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
      );

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
      };
}
