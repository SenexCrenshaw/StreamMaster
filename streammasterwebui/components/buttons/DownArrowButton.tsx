import BaseButton, { type ChildButtonProps } from './BaseButton'

const DownArrowButton: React.FC<ChildButtonProps> = ({
  disabled = false,
  onClick,
  tooltip = 'Add Additional Channels',
}) => {
  return (
    <BaseButton
      disabled={disabled}
      icon="pi-chevron-down"
      iconFilled={false}
      onClick={onClick}
      severity="success"
      tooltip={tooltip}
    />
  )
}

export default DownArrowButton
