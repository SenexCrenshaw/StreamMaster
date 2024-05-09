import SMButton from '@components/sm/SMButton';
import { useSelectedStreamGroup } from '@lib/redux/hooks/selectedStreamGroup';
import { useShowHidden } from '@lib/redux/hooks/showHidden';
import { useCallback, useMemo } from 'react';

interface SMTriSelectShowSGProperties {
  readonly dataKey: string;
  onChange?: (value: boolean | null) => void;
}

export const SMTriSelectShowSG = ({ dataKey, onChange }: SMTriSelectShowSGProperties) => {
  const { showHidden, setShowHidden } = useShowHidden(dataKey);
  const { selectedStreamGroup } = useSelectedStreamGroup('StreamGroup');
  const getToolTip = useMemo((): string => {
    if (showHidden === null) {
      return 'All';
    }

    if (showHidden === true) {
      return 'In ' + selectedStreamGroup?.Name ?? 'SG';
    }

    return 'Not in ' + selectedStreamGroup?.Name ?? 'SG';
  }, [selectedStreamGroup?.Name, showHidden]);

  const moveNext = useCallback(() => {
    if (showHidden === null) {
      setShowHidden(true);
      onChange && onChange(true);
      return;
    }

    if (showHidden === true) {
      setShowHidden(false);
      onChange && onChange(false);
      return;
    }

    setShowHidden(null);
    onChange && onChange(null);
  }, [onChange, setShowHidden, showHidden]);

  const getIcon = useMemo(() => {
    if (showHidden === null) {
      return 'pi pi-list-check';
    }

    if (showHidden === true) {
      return 'pi pi-list-check';
    }

    return 'pi pi-list-check';
  }, [showHidden]);

  const getColor = useMemo(() => {
    if (showHidden === null) {
      return 'icon-yellow';
    }

    if (showHidden === true) {
      return 'icon-green';
    }

    return 'icon-red';
  }, [showHidden]);

  return <SMButton icon={getIcon} iconFilled={false} className={getColor} onClick={() => moveNext()} rounded tooltip={getToolTip} />;
};
