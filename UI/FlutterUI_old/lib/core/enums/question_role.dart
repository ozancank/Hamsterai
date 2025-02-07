enum QuestionRole {
  question('Question'),
  similarQuestion('SimilarQuestion');

  final String role;
  const QuestionRole(this.role);

  @override
  String toString() => name;
}
