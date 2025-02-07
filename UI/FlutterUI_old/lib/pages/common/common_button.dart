import 'package:flutter/material.dart';

// ignore: must_be_immutable
class CommonButton extends StatelessWidget {
  final String text;
  final Color? bgcolor;
  final Color? textColor;
  void Function()? onTap;
  CommonButton({
    super.key,
    this.text = 'Giriş',
    this.bgcolor = Colors.white,
    required this.textColor,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: AnimatedContainer(
        duration: const Duration(seconds: 1),
        margin: const EdgeInsets.symmetric(vertical: 10),
        padding: const EdgeInsets.symmetric(vertical: 15, horizontal: 10),
        alignment: Alignment.center,
        decoration: BoxDecoration(
          color: bgcolor ?? Colors.white,
          borderRadius: BorderRadius.circular(40),
          boxShadow: [
            BoxShadow(
              offset: const Offset(0, 26),
              blurRadius: 60,
              spreadRadius: 0,
              color: const Color(0xFF8D9BAA).withOpacity(0.2),
            ),
          ],
        ),
        child: Text(
          text,
          style: Theme.of(context).textTheme.bodyLarge!.copyWith(
                color: textColor ?? const Color(0xFF838FA0),
                fontWeight: FontWeight.w600,
              ),
        ),
      ),
    );
  }
}
