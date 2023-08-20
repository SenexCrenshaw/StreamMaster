import BaseButton from "./BaseButton";

type DeleteButtonProps = {
  iconFilled?: boolean;
  onClick: () => void;
  tooltip?: string;
}

const DeleteButton: React.FC<DeleteButtonProps> = ({ iconFilled = true, onClick, tooltip = '' }) => {
  return (
    <BaseButton
      icon="pi pi-check"
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
