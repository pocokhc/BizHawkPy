import logging
from typing import Union, cast

logger = logging.getLogger(__name__)


def print_logger(level: Union[int, str] = logging.INFO, log_name: str = "") -> None:
    if isinstance(level, str):
        level = cast(int, logging.getLevelName(level.upper()))

    logger = logging.getLogger(log_name)
    logger.setLevel(level)
    fmt = "%(asctime)s %(name)s %(funcName)s %(lineno)d [%(levelname)s] %(message)s"
    formatter = logging.Formatter(fmt)

    h = logging.StreamHandler()
    h.setLevel(level)
    h.setFormatter(formatter)
    logger.addHandler(h)
