import 'dart:convert';

LessonResponseModel lessonResponseModelFromJson(String str) =>
    LessonResponseModel.fromJson(json.decode(str));

String lessonResponseModelToJson(LessonResponseModel data) =>
    json.encode(data.toJson());

class LessonResponseModel {
  List<Lessons> items;
  int index;
  int size;
  int count;
  int pages;
  bool hasPrevious;
  bool hasNext;

  LessonResponseModel({
    required this.items,
    required this.index,
    required this.size,
    required this.count,
    required this.pages,
    required this.hasPrevious,
    required this.hasNext,
  });

  factory LessonResponseModel.fromJson(Map<String, dynamic> json) =>
      LessonResponseModel(
        items:
            List<Lessons>.from(json["items"].map((x) => Lessons.fromJson(x))),
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

class Lessons {
  int id;
  bool isActive;
  int createUser;
  DateTime createDate;
  int updateUser;
  DateTime updateDate;
  String name;
  int type;

  Lessons({
    required this.id,
    required this.isActive,
    required this.createUser,
    required this.createDate,
    required this.updateUser,
    required this.updateDate,
    required this.name,
    required this.type,
  });

  factory Lessons.fromJson(Map<String, dynamic> json) => Lessons(
      id: json["id"],
      isActive: json["isActive"],
      createUser: json["createUser"],
      createDate: DateTime.parse(json["createDate"]),
      updateUser: json["updateUser"],
      updateDate: DateTime.parse(json["updateDate"]),
      name: json["name"],
      type: json["type"]);

  Map<String, dynamic> toJson() => {
        "id": id,
        "isActive": isActive,
        "createUser": createUser,
        "createDate": createDate.toIso8601String(),
        "updateUser": updateUser,
        "updateDate": updateDate.toIso8601String(),
        "name": name,
        "type": type,
      };
}
