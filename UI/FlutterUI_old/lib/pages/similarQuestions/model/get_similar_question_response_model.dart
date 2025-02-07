import 'dart:convert';

GetSimilarQuestionResponseModel getSimilarQuestionResponseModelFromJson(
        String str) =>
    GetSimilarQuestionResponseModel.fromJson(json.decode(str));

String getSimilarQuestionResponseModelToJson(
        GetSimilarQuestionResponseModel data) =>
    json.encode(data.toJson());

class GetSimilarQuestionResponseModel {
  List<SimilarQuestion> items;
  int index;
  int size;
  int count;
  int pages;
  bool hasPrevious;
  bool hasNext;

  GetSimilarQuestionResponseModel({
    required this.items,
    required this.index,
    required this.size,
    required this.count,
    required this.pages,
    required this.hasPrevious,
    required this.hasNext,
  });

  factory GetSimilarQuestionResponseModel.fromJson(Map<String, dynamic> json) =>
      GetSimilarQuestionResponseModel(
        items: List<SimilarQuestion>.from(
            json["items"].map((x) => SimilarQuestion.fromJson(x))),
        index: json["index"],
        size: json["size"],
        count: json["count"],
        pages: json["pages"],
        hasPrevious: json["hasPrevious"],
        hasNext: json["hasNext"],
      );

  Map<String, dynamic> toJson() => {
        "items": List<dynamic>.from(items.map((x) => x.toJson())),
        "index": index,
        "size": size,
        "count": count,
        "pages": pages,
        "hasPrevious": hasPrevious,
        "hasNext": hasNext,
      };
}

class SimilarQuestion {
  String id;
  int createUser;
  DateTime createDate;
  int lessonId;
  String questionPictureFileName;
  String questionPictureExtension;
  String responseQuestion;
  String responseQuestionFileName;
  String responseQuestionExtension;
  String responseAnswer;
  String responseAnswerFileName;
  String responseAnswerExtension;
  int status;
  String lessonName;

  SimilarQuestion({
    required this.id,
    required this.createUser,
    required this.createDate,
    required this.lessonId,
    required this.questionPictureFileName,
    required this.questionPictureExtension,
    required this.responseQuestion,
    required this.responseQuestionFileName,
    required this.responseQuestionExtension,
    required this.responseAnswer,
    required this.responseAnswerFileName,
    required this.responseAnswerExtension,
    required this.status,
    required this.lessonName,
  });

  factory SimilarQuestion.fromJson(Map<String, dynamic> json) =>
      SimilarQuestion(
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
        "createDate": createDate.toIso8601String(),
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
