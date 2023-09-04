import { type ChildButtonProps } from "./BaseButton";
import BaseButton from "./BaseButton";

const DeleteButton: React.FC<ChildButtonProps> = ({ disabled = false, iconFilled, label, onClick, tooltip = 'Delete Stream' }) => {
  return (
    <BaseButton
      disabled={disabled}
      icon="pi-minus"
      iconFilled={iconFilled}
      label={iconFilled === true ? undefined : label}
      onClick={onClick}
      severity="danger"
      tooltip={tooltip}
    />
  );
};

export default DeleteButton;
