import 'package:flutter/material.dart';
import 'package:lottie/lottie.dart';

class NoConnection extends StatefulWidget {
  const NoConnection({super.key});

  @override
  State<NoConnection> createState() => _NoConnectionState();
}

class _NoConnectionState extends State<NoConnection> {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      body: Center(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.center,
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Lottie.asset(
              'assets/json/no_connection.json',
            ),
            const Text(
              "İnternet bağlantısı yok!",
              style: TextStyle(fontSize: 20),
            ),
          ],
        ),
      ),
    );
  }
}
