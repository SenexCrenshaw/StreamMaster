import React from 'react';
import { motion } from 'framer-motion';

interface AnimatedTextProps {
  text: string;
  fontSize?: number;
  color?: string;
  waveDirection?: 'leftToRight' | 'rightToLeft';
}

const AnimatedText: React.FC<AnimatedTextProps> = ({ text, fontSize = 18, color = 'black', waveDirection = 'leftToRight' }) => {
  const letters = text.split('');

  const containerVariants = {
    hidden: { opacity: 1 },
    visible: {
      opacity: 1,
      transition: {
        staggerChildren: 0.05,
        when: waveDirection === 'rightToLeft' ? 'afterChildren' : 'beforeChildren'
      }
    }
  };

  const letterVariants = {
    hidden: {
      opacity: 0,
      y: 50
    },
    visible: {
      opacity: 1,
      y: 0,
      transition: {
        ease: 'easeOut',
        duration: 0.4
      }
    }
  };

  return (
    <motion.div style={{ display: 'flex', overflow: 'hidden', fontSize, color }} variants={containerVariants} initial="hidden" animate="visible">
      {waveDirection === 'rightToLeft'
        ? letters.reverse().map((letter, index) => (
            <motion.span key={index} variants={letterVariants}>
              {letter}
            </motion.span>
          ))
        : letters.map((letter, index) => (
            <motion.span key={index} variants={letterVariants}>
              {letter}
            </motion.span>
          ))}
    </motion.div>
  );
};

export default AnimatedText;
