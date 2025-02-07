import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_svg/svg.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/styles/colors.dart';

// ignore: must_be_immutable
class CommonTextField extends StatefulWidget {
  CommonTextField(
      {super.key,
      required this.controller,
      required this.hintText,
      required this.iconPath,
      this.isObsecure = false,
      required this.svgColor,
      this.isPassword = false,
      this.keyboardType,
      this.labelTextColor});

  final TextEditingController controller;
  final String hintText;
  final String iconPath;
  bool isObsecure;
  final Color svgColor;
  final bool isPassword;
  final TextInputType? keyboardType;
  final Color? labelTextColor;

  @override
  State<CommonTextField> createState() => _CommonTextFieldState();
}

class _CommonTextFieldState extends State<CommonTextField> {
  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.symmetric(vertical: 10),
      padding: const EdgeInsets.all(10),
      decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(40),
          boxShadow: [
            BoxShadow(
              offset: const Offset(0, 12),
              blurRadius: 60,
              spreadRadius: 0,
              color: const Color(0xFF43474D).withOpacity(0.06),
            )
          ]),
      child: Row(
        children: [
          SvgPicture.asset(
            widget.iconPath,
            height: context.dynamicHeight * 0.045,
            // ignore: deprecated_member_use
            color: widget.svgColor,
          ),
          SizedBox(
            width: context.dynamicWidth * 0.04,
          ),
          Flexible(
            child: TextFormField(
              controller: widget.controller,
              keyboardType:
                  widget.keyboardType ?? TextInputType.visiblePassword,
              decoration: InputDecoration(
                border: InputBorder.none,
                hintText: widget.hintText,
                hintStyle: Theme.of(context).textTheme.bodyLarge!.copyWith(
                      color: MyColors.hintColor,
                    ),
              ),
              style: Theme.of(context)
                  .textTheme
                  .bodyLarge!
                  .copyWith(color: widget.labelTextColor ?? Colors.black),
              obscureText: widget.isObsecure,
              obscuringCharacter: '●',
            ),
          ),
          if (widget.isPassword == true && widget.controller.text.isNotEmpty)
            GestureDetector(
              onTap: () {
                HapticFeedback.mediumImpact();
                setState(() {
                  widget.isObsecure = !widget.isObsecure;
                });
              },
              child: Padding(
                padding: const EdgeInsets.symmetric(horizontal: 10),
                child: SvgPicture.asset(AssetsConstant.eyeOn),
              ),
            )
          else
            const SizedBox(),
        ],
      ),
    );
  }
}
