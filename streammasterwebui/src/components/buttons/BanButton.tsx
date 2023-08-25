;

import { type ChildButtonProps } from "./BaseButton";
import BaseButton from "./BaseButton";

const BanButton: React.FC<ChildButtonProps> = ({ disabled = false, onClick, tooltip = "" }) => {
  return (
    <BaseButton
      disabled={disabled}
      icon="pi-ban"
      iconFilled={false}
      onClick={onClick}
      tooltip={tooltip}
    />
  );
};

export default BanButton;
