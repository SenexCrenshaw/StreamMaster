export function streamsBodyTemplate(activeCount: string, totalCount: string) {
  if (activeCount === null || totalCount === undefined) {
    return null;
  }

  return (
    <div className="flex align-items-center gap-1">
      {activeCount}/{totalCount}
    </div>
  );
}
