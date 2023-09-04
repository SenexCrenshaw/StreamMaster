import { type ChildButtonProps } from "./BaseButton";
import BaseButton from "./BaseButton";

const VisibleButton: React.FC<ChildButtonProps> = ({ disabled = true, iconFilled, onClick, tooltip = "Set Visibility" }) => {
  return (
    <BaseButton
      disabled={disabled}
      icon="pi-eye-slash"
      iconFilled={iconFilled}
      onClick={onClick}
      tooltip={tooltip}
    />
  );
};

export default VisibleButton;
