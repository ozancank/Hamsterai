class LessonDataModel {
  final Map<String, int> forLessons;
  final Map<String, int> forGains;
  final Map<String, Map<String, int>> forLessonGains;

  LessonDataModel({
    required this.forLessons,
    required this.forGains,
    required this.forLessonGains,
  });

  factory LessonDataModel.fromJson(Map<String, dynamic> json) {
    return LessonDataModel(
      forLessons: Map<String, int>.from(json['forLessons']),
      forGains: Map<String, int>.from(json['forGains']),
      forLessonGains: (json['forLessonGains'] as Map<String, dynamic>).map(
        (lesson, gains) => MapEntry(
          lesson,
          Map<String, int>.from(gains),
        ),
      ),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'forLessons': forLessons,
      'forGains': forGains,
      'forLessonGains': forLessonGains,
    };
  }
}
