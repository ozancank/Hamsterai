import 'package:flutter/material.dart';

class DrawingPainter extends CustomPainter {
  final List<Offset?> points;
  final Color color;
  final double strokeWidth;
  final bool isEraserMode;

  DrawingPainter(this.points, this.color, this.strokeWidth, this.isEraserMode);

  @override
  void paint(Canvas canvas, Size size) {
    Paint paint = Paint()
      ..color = isEraserMode ? Colors.transparent : color
      ..strokeCap = StrokeCap.round
      ..strokeWidth = strokeWidth;

    for (int i = 0; i < points.length - 1; i++) {
      if (points[i] != null && points[i + 1] != null) {
        canvas.drawLine(points[i]!, points[i + 1]!, paint);
      }
    }
  }

  @override
  bool shouldRepaint(covariant DrawingPainter oldDelegate) {
    return true;
  }
}