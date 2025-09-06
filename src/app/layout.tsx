import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";

const inter = Inter({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "Creative Puzzle Adventure - Unity Game",
  description: "Game puzzle kreatif Unity 2D untuk anak-anak dengan drag & drop mechanics, mode kreativitas, dan sistem reward yang menyenangkan.",
  keywords: "unity game, puzzle game, children games, creative drawing, educational games, mobile games",
  openGraph: {
    title: "Creative Puzzle Adventure",
    description: "Game edukatif Unity 2D dengan puzzle logika dan mode kreativitas untuk anak-anak",
    type: "website",
  },
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="id" className="scroll-smooth">
      <head>
        <meta name="viewport" content="width=device-width, initial-scale=1" />
        <meta name="theme-color" content="#3B82F6" />
        <link rel="icon" href="/favicon.ico" />
      </head>
      <body className={`${inter.className} antialiased`}>
        <div className="flex flex-col min-h-screen">
          {children}
        </div>
      </body>
    </html>
  );
}