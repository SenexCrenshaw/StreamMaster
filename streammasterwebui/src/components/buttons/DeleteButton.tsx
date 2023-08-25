import { type ChildButtonProps } from "./BaseButton";
import BaseButton from "./BaseButton";


const DeleteButton: React.FC<ChildButtonProps> = ({ iconFilled = true, onClick, tooltip = '' }) => {
  return (
    <BaseButton
      icon="pi-minus"
      iconFilled={iconFilled}
      label="Delete"
      onClick={onClick}
      rounded
      severity="danger"
      tooltip={tooltip}
    />
  );
};

export default DeleteButton;
