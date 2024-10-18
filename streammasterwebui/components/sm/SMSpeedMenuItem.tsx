import React from 'react';
import { motion } from 'framer-motion';

interface SMSpeedMenuItemProps {
  icon?: string; // URL, relative path, or class name
  command?: () => void;
  animateOn?: 'hover' | 'click';
  url?: string; // Optional URL to open on click
  template?: React.ReactNode; // Optional custom template
}

const SMSpeedMenuItem: React.FC<SMSpeedMenuItemProps> = ({ icon, command, animateOn, url, template }) => {
  const handleClick = () => {
    if (url) {
      window.open(url, '_blank');
    } else if (command) {
      command();
    }
  };

  return (
    <motion.div
      className="smspeed-menu-item"
      onClick={template ? undefined : handleClick} // Only handle click if no template is provided
      whileHover={animateOn === 'hover' ? { scale: 1.1 } : {}}
      whileTap={animateOn === 'click' ? { scale: 0.9 } : {}}
    >
      {template ? (
        template
      ) : icon ? (
        icon.startsWith('http') || icon.startsWith('/') ? (
          <img src={icon} alt="icon" className="smspeed-menu-icon" />
        ) : (
          <i className={icon}></i>
        )
      ) : null}
    </motion.div>
  );
};

export default SMSpeedMenuItem;
