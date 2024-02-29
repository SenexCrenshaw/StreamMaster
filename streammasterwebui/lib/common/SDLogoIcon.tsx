import React from 'react';
import sdLogoImage from '../../public/images/sd_logo.png'; // Adjust the path as necessary

interface SDLogoIconProps {
  size?: number;
  [x: string]: any; // Additional props
}

const SDLogoIcon: React.FC<SDLogoIconProps> = ({ size = 24, ...props }) => (
  <img src={sdLogoImage} alt="Custom Icon" style={{ width: size, height: size }} {...props} />
);

export default SDLogoIcon;
