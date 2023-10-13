import { getTopToolOptions } from '@lib/common/common';
import { useShowHidden } from '@lib/redux/slices/useShowHidden';
import { TriStateCheckbox, type TriStateCheckboxChangeEvent } from 'primereact/tristatecheckbox';

type TriSelectProps = {
  readonly dataKey: string;
};
export const TriSelectShowHidden = ({ dataKey }: TriSelectProps) => {
  const { showHidden, setShowHidden } = useShowHidden(dataKey);

  const getToolTip = (value: boolean | null | undefined): string => {
    if (value === null) {
      return 'Show All';
    }

    if (value === true) {
      return 'Show Visible';
    }

    return 'Show Hidden';
  };

  return (
    <TriStateCheckbox
      className="sm-tristatecheckbox"
      onChange={(e: TriStateCheckboxChangeEvent) => {
        setShowHidden(e.value);
      }}
      tooltip={getToolTip(showHidden)}
      tooltipOptions={getTopToolOptions}
      value={showHidden}
    />
  );
};
