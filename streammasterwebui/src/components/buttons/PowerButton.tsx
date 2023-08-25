;

import { type ChildButtonProps } from "./BaseButton";
import BaseButton from "./BaseButton";

const PowerButton: React.FC<ChildButtonProps> = ({ disabled = true, onClick, tooltip = "Set Visibility" }) => {
  return (
    <BaseButton
      disabled={disabled}
      icon="pi-power-off"
      onClick={onClick}
      tooltip={tooltip}
    />
  );
};

export default PowerButton;
