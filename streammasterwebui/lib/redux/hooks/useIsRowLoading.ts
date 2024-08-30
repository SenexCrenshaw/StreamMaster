import { Logger } from '@lib/common/logger';
import { useAppDispatch, useAppSelector } from '../hooks';
import { RootState } from '../store';
import { selectIsRowLoading, setRowLoading } from './loading';
import { useMemo, useCallback } from 'react';

interface UseIsRowLoadingProps {
  Entity: string;
  Id: string;
}

const useIsRowLoading = ({ Entity, Id }: UseIsRowLoadingProps): [boolean, (isLoading: boolean) => void] => {
  const dispatch = useAppDispatch();

  const isRowLoading = useAppSelector(useMemo(() => (state: RootState) => selectIsRowLoading(state, Entity, Id), [Entity, Id]));

  const setIsRowLoading = useCallback(
    (isLoading: boolean) => {
      Logger.debug(`Setting row loading for ${Entity} ${Id} to ${isLoading}`);
      dispatch(setRowLoading({ entity: Entity, id: Id, isLoading }));
    },
    [Entity, Id, dispatch]
  );

  return [isRowLoading, setIsRowLoading];
};

export default useIsRowLoading;
