import 'dart:ui';

import 'package:flutter/material.dart';

class VerticalLineWithArrow extends StatelessWidget {
  final int itemCount; // Kaç kazanım varsa ona göre çizgi uzayacak
  final Color color;

  const VerticalLineWithArrow({
    super.key,
    required this.itemCount,
    required this.color,
  });

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 30, // Çizginin genişliği
      child: CustomPaint(
        size: Size(30, (itemCount * 60)), // Yüksekliği her satır için 60px ayarladım
        painter: LineArrowPainter(itemCount: itemCount, color: color),
      ),
    );
  }
}

class LineArrowPainter extends CustomPainter {
  final int itemCount;
  final Color color;

  LineArrowPainter({required this.itemCount, required this.color});

  @override
  void paint(Canvas canvas, Size size) {
    Paint paint = Paint()
      ..color = color
      ..strokeWidth = 1.5;

    double lineLength = itemCount * 60.0; // Her bir satır için 60px uzatma

    // Dikey çizgi
    canvas.drawLine(Offset(size.width / 2, 0), Offset(size.width / 2, lineLength), paint);

    // Son noktaya yuvarlak nokta ekleyelim
    canvas.drawCircle(Offset(size.width / 2, lineLength), 3, paint);

    // Her kazanım için ok ekleyelim
    for (int i = 0; i < itemCount; i++) {
      double yPosition = i * 60.0 + 30; // Her satır için ortasına ok ekliyoruz
      _drawArrow(canvas, size.width / 2, yPosition, paint);
    }
  }

  void _drawArrow(Canvas canvas, double x, double y, Paint paint) {
    // Ok için küçük bir çizgi ve ok başı
    canvas.drawLine(Offset(x, y), Offset(x + 10, y), paint);
    canvas.drawLine(Offset(x + 10, y), Offset(x + 5, y - 5), paint);
    canvas.drawLine(Offset(x + 10, y), Offset(x + 5, y + 5), paint);
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) {
    return false;
  }
}
