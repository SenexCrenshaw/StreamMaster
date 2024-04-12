import { getTopToolOptions } from '@lib/common/common';
import { useShowHidden } from '@lib/redux/slices/useShowHidden';
import { TriStateCheckbox, type TriStateCheckboxChangeEvent } from 'primereact/tristatecheckbox';
import { useMemo, useRef } from 'react';

interface TriSelectProperties {
  readonly dataKey: string;
}
export const TriSelectShowHidden = ({ dataKey }: TriSelectProperties) => {
  const { showHidden, setShowHidden } = useShowHidden(dataKey);
  const ref = useRef<TriStateCheckbox>(null);
  const getToolTip = useMemo((): string => {
    if (showHidden === null) {
      return 'All';
    }

    if (showHidden === true) {
      return 'Visible';
    }

    return 'Hidden';
  }, [showHidden]);

  return (
    <TriStateCheckbox
      uncheckIcon="pi pi-eye-slash"
      checkIcon="pi pi-eye"
      className="sm-tristatecheckbox"
      onChange={(e: TriStateCheckboxChangeEvent) => {
        setShowHidden(e.value);
      }}
      tooltip={getToolTip}
      tooltipOptions={getTopToolOptions}
      value={showHidden}
      pt={
        {
          // input:{ }
        }
      }
      variant="outlined"
    />
  );
};
