import 'package:flutter/material.dart';
import 'dart:math';

bool isTablet(BuildContext context) {
  final size = MediaQuery.of(context).size;
  final diagonal = sqrt(size.width * size.width + size.height * size.height);
  return size.shortestSide >= 600;
}
