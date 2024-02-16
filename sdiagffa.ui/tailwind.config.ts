import type { Config } from "tailwindcss";
import plugin from "tailwindcss/plugin";

const starwarsCrawlPerspective = plugin(({ addUtilities }) => {
  addUtilities({
    '.swcrawl-text': {
      transform: 'rotateX(50deg) scale(1.1)'
    },
    '.swcrawl-container': {
      'perspective': '900px'
    }
  })
})

const config: Config = {
  content: [
    "./src/pages/**/*.{js,ts,jsx,tsx,mdx}",
    "./src/components/**/*.{js,ts,jsx,tsx,mdx}",
    "./src/app/**/*.{js,ts,jsx,tsx,mdx}",
  ],
  theme: {
    colors: {
      'swblue': '#3C73AE',
      'swblack': '#1B1614',
      'swyellow': '#FFD21B',
      'swgray': '#D7DFE7',
      'white': '#FFFFFF'
    },
    extend: {
      keyframes: {
        flipfade: {
          '0%': {
            transform: 'rotateY(0deg)',
            opacity: '0'
          },
          '25%': {
            opacity: '1'
          },
          '50%': {
            opacity: '0'
          },
          '75%': {
            opacity: '1'
          },
          '100%': {
            transform: 'rotateY(1080deg)',
            opacity: '0'
          },
        }
      },
      animation: {
        flipfade: 'flipfade 2s linear infinite'
      }
    },
  },
  plugins: [
    starwarsCrawlPerspective
  ],
};
export default config;
