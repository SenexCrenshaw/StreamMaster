import { type ChildButtonProps } from "./BaseButton";
import BaseButton from "./BaseButton";

const ResetButton: React.FC<ChildButtonProps> = ({ disabled = false, onClick, tooltip = "" }) => {
  return (
    <BaseButton
      disabled={disabled}
      icon="pi-history"
      iconFilled={false}
      onClick={onClick}
      tooltip={tooltip}
    />
  );
};

export default ResetButton;
