import 'dart:convert';

QuizListModel quizListModelFromJson(String str) =>
    QuizListModel.fromJson(json.decode(str));

String quizListModelToJson(QuizListModel data) => json.encode(data.toJson());

class QuizListModel {
  List<QuizItem> items;
  int index;
  int size;
  int count;
  int pages;
  bool hasPrevious;
  bool hasNext;

  QuizListModel({
    required this.items,
    required this.index,
    required this.size,
    required this.count,
    required this.pages,
    required this.hasPrevious,
    required this.hasNext,
  });

  factory QuizListModel.fromJson(Map<String, dynamic> json) => QuizListModel(
        items:
            List<QuizItem>.from(json["items"].map((x) => QuizItem.fromJson(x))),
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

class QuizItem {
  String id;
  int userId;
  int lessonId;
  int timeSecond;
  int status;
  int correctCount;
  int wrongCount;
  int emptyCount;
  int successRate;
  int questionCount;
  String userFullName;
  dynamic schoolName;
  String lessonName;
  List<String> gainNames;

  QuizItem({
    required this.id,
    required this.userId,
    required this.lessonId,
    required this.timeSecond,
    required this.status,
    required this.correctCount,
    required this.wrongCount,
    required this.emptyCount,
    required this.successRate,
    required this.questionCount,
    required this.userFullName,
    required this.schoolName,
    required this.lessonName,
    required this.gainNames,
  });

  factory QuizItem.fromJson(Map<String, dynamic> json) => QuizItem(
        id: json["id"],
        userId: json["userId"],
        lessonId: json["lessonId"],
        timeSecond: json["timeSecond"],
        status: json["status"],
        correctCount: json["correctCount"],
        wrongCount: json["wrongCount"],
        emptyCount: json["emptyCount"],
        successRate: json["successRate"],
        questionCount: json["questionCount"],
        userFullName: json["userFullName"],
        schoolName: json["schoolName"],
        lessonName: json["lessonName"],
        gainNames: List<String>.from(json["gainNames"].map((x) => x ?? "")),
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
        "questionCount": questionCount,
        "userFullName": userFullName,
        "schoolName": schoolName,
        "lessonName": lessonName,
        "gainNames": List<dynamic>.from(gainNames.map((x) => x)),
      };
}
