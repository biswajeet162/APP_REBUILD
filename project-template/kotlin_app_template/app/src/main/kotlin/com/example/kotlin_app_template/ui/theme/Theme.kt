package com.example.kotlin_app_template.ui.theme

import androidx.compose.foundation.isSystemInDarkTheme
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.darkColorScheme
import androidx.compose.material3.lightColorScheme
import androidx.compose.runtime.Composable

private val DarkColorScheme = darkColorScheme(
    primary = GreenLight,
    secondary = GreenPrimary,
    tertiary = GreenDark,
)

private val LightColorScheme = lightColorScheme(
    primary = GreenDark,
    secondary = GreenPrimary,
    tertiary = GreenLight,
)

@Composable
fun KotlinAppTemplateTheme(
    darkTheme: Boolean = isSystemInDarkTheme(),
    content: @Composable () -> Unit,
) {
    val colorScheme = if (darkTheme) DarkColorScheme else LightColorScheme

    MaterialTheme(
        colorScheme = colorScheme,
        typography = Typography,
        content = content,
    )
}
