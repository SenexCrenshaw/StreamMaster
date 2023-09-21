import { type ChildButtonProps } from "./BaseButton";
import BaseButton from "./BaseButton";

const AutoSetButton: React.FC<ChildButtonProps> = ({ disabled = true, onClick, tooltip = "Auto Set" }) => {
  return (
    <BaseButton
      disabled={disabled}
      icon="pi-sort-numeric-up-alt"
      onClick={onClick}
      tooltip={tooltip}
    />
  );
};

export default AutoSetButton;
