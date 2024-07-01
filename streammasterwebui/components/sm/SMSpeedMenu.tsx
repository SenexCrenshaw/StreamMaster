import React, { useState, useRef } from 'react';
import { motion } from 'framer-motion';
import SMSpeedMenuItem from './SMSpeedMenuItem';
import { calculateChildPosition } from './helpers/calculateChildPosition';
import { calculateBackgroundStyle } from './helpers/calculateBackgroundStyle';
import { useClickOutside } from './hooks/useClickOutside';
import { Shape, Direction } from '@components/sm/Interfaces/SMSpeedDialTypes'; // Import the types

interface SMSpeedMenuProps {
  mainItem: { icon: string; command: () => void; direction?: Direction; shape: Shape; animateOn?: 'hover' | 'click'; modal?: boolean };
  items: { icon?: string; command?: () => void; animateOn?: 'hover' | 'click'; url?: string; template?: React.ReactNode }[];
  backgroundWidth?: string; // New optional prop for manually setting background width
}

const SMSpeedMenu: React.FC<SMSpeedMenuProps> = ({ mainItem, items, backgroundWidth }) => {
  const [isOpen, setIsOpen] = useState(false);
  const menuRef = useRef<HTMLDivElement>(null);

  useClickOutside(menuRef, () => setIsOpen(false));

  const toggleMenu = () => {
    setIsOpen(!isOpen);
  };

  const handleMouseEnter = () => {
    if (mainItem.animateOn === 'hover') {
      setIsOpen(true);
    }
  };

  const handleMouseLeave = () => {
    if (mainItem.animateOn === 'hover') {
      setIsOpen(false);
    }
  };

  const { direction, shape, animateOn } = mainItem;

  return (
    <div
      className="smspeed-menu-container"
      ref={menuRef}
      onClick={animateOn === 'click' ? toggleMenu : undefined}
      onMouseEnter={animateOn === 'hover' ? handleMouseEnter : undefined}
      onMouseLeave={animateOn === 'hover' ? handleMouseLeave : undefined}
      style={{ display: 'inline-block', position: 'relative' }}
    >
      {isOpen && (
        <div className="smspeed-menu-background" style={calculateBackgroundStyle(isOpen, shape, direction as Direction, items.length, backgroundWidth)}></div>
      )}
      <motion.div className="smspeed-main-item" style={{ position: 'relative', zIndex: 1 }}>
        <SMSpeedMenuItem icon={mainItem.icon} command={mainItem.command} animateOn={animateOn} />
        {isOpen &&
          items.map((item, index) => {
            const position = calculateChildPosition(index, items.length, shape, direction);
            return (
              <motion.div
                key={index}
                className="smspeed-menu-item-wrapper"
                style={{ position: 'absolute', left: `${position.x}px`, top: `${position.y}px` }}
                initial={{ opacity: 0 }}
                animate={{ opacity: 1 }}
                transition={{ delay: 0.2 * index }}
              >
                <SMSpeedMenuItem icon={item.icon} command={item.command} animateOn={item.animateOn} url={item.url} template={item.template} />
              </motion.div>
            );
          })}
      </motion.div>
    </div>
  );
};

export default SMSpeedMenu;
