import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_svg/svg.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/styles/colors.dart';

class ProfileTextField extends StatefulWidget {
  final TextEditingController controller;
  final String hintText;
  final bool isObsecure;
  final bool isGrey;
  final TextInputType keyboardType;
  final String? Function(String?)? validator;
  final VoidCallback? onTap;
  final bool isPassword;
  final bool enabled;
  const ProfileTextField({
    super.key,
    required this.controller,
    required this.hintText,
    this.isObsecure = false,
    this.isGrey = false,
    this.keyboardType = TextInputType.name,
    required this.validator,
    this.onTap,
    this.isPassword = false,
    this.enabled = true,
  });

  @override
  State<ProfileTextField> createState() => _ProfileTextFieldState();
}

class _ProfileTextFieldState extends State<ProfileTextField> {
  late bool _isObsecure;
  late TextInputType _keyboardType;

  @override
  void initState() {
    super.initState();
    _isObsecure = widget.isObsecure;
    _keyboardType = widget.keyboardType;
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: EdgeInsets.symmetric(
        vertical: 10,
        horizontal: context.dynamicWidth * 0.05,
      ),
      padding: const EdgeInsets.all(5),
      decoration: BoxDecoration(
        color: widget.isGrey
            ? const Color(0xFFADB4C0).withOpacity(0.3)
            : Colors.white,
        borderRadius: BorderRadius.circular(40),
        border: Border.all(color: const Color(0xFFADB4C0).withOpacity(0.5)),
        boxShadow: [
          BoxShadow(
            offset: const Offset(0, 12),
            blurRadius: 60,
            spreadRadius: 0,
            color: const Color(0xFF43474D).withOpacity(0.06),
          )
        ],
      ),
      child: Padding(
        padding: EdgeInsets.symmetric(horizontal: context.dynamicWidth * 0.05),
        child: Row(
          children: [
            Flexible(
              child: TextFormField(
                controller: widget.controller,
                keyboardType: _keyboardType,
                obscureText: widget.isObsecure,
                enabled: widget.enabled,
                obscuringCharacter: '●',
                validator: widget.validator,
                style: Theme.of(context).textTheme.bodyLarge,
                decoration: InputDecoration(
                  border: InputBorder.none,
                  hintText: widget.hintText,
                  hintStyle: Theme.of(context)
                      .textTheme
                      .bodyLarge!
                      .copyWith(color: MyColors.primaryColor),
                ),
              ),
            ),
            widget.isPassword
                ? GestureDetector(
                    onTap: widget.onTap,
                    child: Padding(
                      padding: const EdgeInsets.symmetric(horizontal: 10),
                      child: SvgPicture.asset(AssetsConstant.eyeOn),
                    ),
                  )
                : const SizedBox(),
          ],
        ),
      ),
    );
  }
}
