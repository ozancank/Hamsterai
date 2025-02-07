import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:mobile/pages/common/common_appbar.dart';
import 'package:mobile/styles/colors.dart';

class SettingsPage extends StatefulWidget {
  const SettingsPage({super.key});

  @override
  State<SettingsPage> createState() => _SettingsPageState();
}

class _SettingsPageState extends State<SettingsPage> {
  bool _isSwitched = true;
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      appBar: const CommonAppbar(
        title: 'Ayarlar',
      ),
      body: Column(
        children: [
          SizedBox(height: context.height * 0.05),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceAround,
            children: [
              Text(
                'Bildirim Gösterimi',
                style: Theme.of(context).textTheme.bodyMedium!.copyWith(
                      color: MyColors.primaryColor,
                      fontWeight: FontWeight.w700,
                    ),
              ),
              Switch(
                activeColor: MyColors.primaryColor,
                value: _isSwitched,
                onChanged: (value) {
                  setState(() {
                    _isSwitched = value;
                  });
                },
              ),
            ],
          )
        ],
      ),
    );
  }
}


// Center(
//         child: Switch(
//           activeColor: MyColors.primaryColor,
//           value: _isSwitched,
//           onChanged: (value) {
//             setState(() {
//               _isSwitched = value;
//             });
//           },
//         ),
//       ),