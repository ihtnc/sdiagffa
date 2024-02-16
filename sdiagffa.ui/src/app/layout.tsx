import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";

const inter = Inter({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "Six Degrees In A Galaxy Far Far Away",
    description: "Find the connection between two Star Wars characters. Based on Bacon's Law (https://en.wikipedia.org/wiki/Six_Degrees_of_Kevin_Bacon)",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <head>
        <link rel="stylesheet" href="/fonts/font-awesome.min.css" />
      </head>
      <body className={inter.className}>{children}</body>
    </html>
  );
}
