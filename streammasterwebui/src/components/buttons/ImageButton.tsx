;

import { type ChildButtonProps } from "./BaseButton";
import BaseButton from "./BaseButton";

const ImageButton: React.FC<ChildButtonProps> = ({ disabled = true, onClick, tooltip = "" }) => {
  return (
    <BaseButton
      disabled={disabled}
      icon="pi-image"
      onClick={onClick}
      tooltip={tooltip}
    />
  );
};

export default ImageButton;
